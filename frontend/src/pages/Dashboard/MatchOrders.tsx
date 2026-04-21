import { FormEvent, useEffect, useState } from "react";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../../components/ui/table";

type MatchItem = {
  id: string;
  home: string;
  away: string;
};

type OrderItem = {
  id: string;
  bookmaker: string;
  snapshotTime: string;
  hoursBefore: number;
  oddsHome: number;
  oddsDraw: number;
  oddsAway: number;
};

type MatchOrderRow = OrderItem & {
  matchId: string;
  matchHome: string;
  matchAway: string;
};

const PAGE_SIZE = 20;
const API_BASE_URL =
  (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ?? "";

export default function MatchOrders() {
  const [orders, setOrders] = useState<MatchOrderRow[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [hasNextPage, setHasNextPage] = useState(false);

  useEffect(() => {
    const controller = new AbortController();

    const fetchJsonArray = async (endpoint: string): Promise<unknown[] | null> => {
      const response = await fetch(endpoint, { signal: controller.signal });
      if (!response.ok) {
        if (response.status === 404) {
          return null;
        }
        throw new Error(`Request failed with status ${response.status}`);
      }

      const contentType = response.headers.get("content-type") ?? "";
      if (!contentType.includes("application/json")) {
        return null;
      }

      const data = (await response.json()) as unknown;
      if (!Array.isArray(data)) {
        return null;
      }

      return data;
    };

    const loadOrders = async () => {
      setLoading(true);
      setError(null);

      try {
        const params = new URLSearchParams({
          page: String(page),
          pageSize: String(PAGE_SIZE),
        });

        if (search.trim()) {
          params.set("search", search.trim());
        }

        // Preferred endpoint for a dedicated orders page.
        const directOrderEndpoints = API_BASE_URL
          ? [
              `${API_BASE_URL}/api/matches/orders?${params.toString()}`,
              `/api/matches/orders?${params.toString()}`,
            ]
          : [`/api/matches/orders?${params.toString()}`];

        for (const endpoint of directOrderEndpoints) {
          const directData = await fetchJsonArray(endpoint);
          if (!directData) {
            continue;
          }

          const rows = (directData as MatchOrderRow[]).map((item) => ({
            ...item,
            matchId: item.matchId ?? "",
            matchHome: item.matchHome ?? "-",
            matchAway: item.matchAway ?? "-",
          }));

          setOrders(rows);
          setHasNextPage(rows.length === PAGE_SIZE);
          setLoading(false);
          return;
        }

        // Fallback for current backend: load matches page then fetch each match's orders.
        const matchesEndpoints = API_BASE_URL
          ? [`${API_BASE_URL}/api/matches?${params.toString()}`, `/api/matches?${params.toString()}`]
          : [`/api/matches?${params.toString()}`];

        let matches: MatchItem[] | null = null;
        for (const endpoint of matchesEndpoints) {
          const matchData = await fetchJsonArray(endpoint);
          if (!matchData) {
            continue;
          }
          matches = matchData as MatchItem[];
          break;
        }

        if (!matches) {
          throw new Error("Matches endpoint not found.");
        }

        const rowsByMatch = await Promise.all(
          matches.map(async (match) => {
            const orderEndpoints = API_BASE_URL
              ? [
                  `${API_BASE_URL}/api/matches/${match.id}/orders`,
                  `/api/matches/${match.id}/orders`,
                ]
              : [`/api/matches/${match.id}/orders`];

            let orderData: unknown[] | null = null;
            for (const endpoint of orderEndpoints) {
              const data = await fetchJsonArray(endpoint);
              if (data) {
                orderData = data;
                break;
              }
            }

            const ordersForMatch = (orderData ?? []) as OrderItem[];
            return ordersForMatch.map((order) => ({
              ...order,
              matchId: match.id,
              matchHome: match.home,
              matchAway: match.away,
            }));
          })
        );

        const flattenedRows = rowsByMatch.flat();
        setOrders(flattenedRows);
        setHasNextPage(matches.length === PAGE_SIZE);
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === "AbortError") {
          return;
        }

        setOrders([]);
        setHasNextPage(false);
        setError("Failed to load orders.");
      } finally {
        setLoading(false);
      }
    };

    void loadOrders();

    return () => {
      controller.abort();
    };
  }, [page, search]);

  const handleSearchSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  return (
    <>
      <PageMeta title="Match Orders | Trading Engine" description="Orders page for Trading Engine." />
      <PageBreadcrumb pageTitle="Match Orders" />

      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="mb-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <form className="flex w-full gap-2 md:max-w-xl" onSubmit={handleSearchSubmit}>
            <input
              type="text"
              value={searchInput}
              onChange={(event) => setSearchInput(event.target.value)}
              placeholder="Search matches..."
              className="h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 text-sm text-gray-800 outline-none focus:border-brand-500 dark:border-gray-700 dark:text-white/90"
            />
            <button
              type="submit"
              className="h-11 rounded-lg bg-brand-500 px-4 text-sm font-medium text-white hover:bg-brand-600"
            >
              Search
            </button>
          </form>

          <div className="text-sm text-gray-500 dark:text-gray-400">Page {page}</div>
        </div>

        {error && (
          <div className="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900/50 dark:bg-red-900/20 dark:text-red-300">
            {error}
          </div>
        )}

        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
              <TableRow>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Match
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Bookmaker
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Snapshot Time
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Hours Before
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Odds (H/D/A)
                </TableCell>
              </TableRow>
            </TableHeader>

            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <TableRow>
                  <TableCell className="px-5 py-6 text-sm text-gray-500 dark:text-gray-400" colSpan={5}>
                    Loading orders...
                  </TableCell>
                </TableRow>
              ) : (
                orders.map((order, index) => (
                  <TableRow key={`${order.matchId}-${order.id}-${index}`}>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                      {order.matchHome} vs {order.matchAway}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                      {order.bookmaker}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {new Date(order.snapshotTime).toLocaleString()}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {order.hoursBefore}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {order.oddsHome} / {order.oddsDraw} / {order.oddsAway}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <button
            type="button"
            onClick={() => setPage((prev) => Math.max(1, prev - 1))}
            disabled={loading || page === 1}
            className="rounded-lg border border-gray-300 px-4 py-2 text-sm text-gray-700 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300"
          >
            Previous
          </button>

          <button
            type="button"
            onClick={() => setPage((prev) => prev + 1)}
            disabled={loading || !hasNextPage}
            className="rounded-lg border border-gray-300 px-4 py-2 text-sm text-gray-700 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:text-gray-300"
          >
            Next
          </button>
        </div>
      </div>
    </>
  );
}