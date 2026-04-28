import { useEffect, useState } from "react";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../../components/ui/table";
import { getAuthHeaders } from "../../utils/auth";

type RegistryItem = {
  id: string;
  polymarketHome: string;
  polymarketAway: string;
  oddsApiHome: string | null;
  oddsApiAway: string | null;
  correlationScore: number | null;
};

const PAGE_SIZE = 20;
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ?? "";

export default function Registry() {
  const [items, setItems] = useState<RegistryItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);

  useEffect(() => {
    const controller = new AbortController();

    const loadRegistryItems = async () => {
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

        const endpointCandidates = API_BASE_URL
          ? [`${API_BASE_URL}/api/registry`, "/api/registry"]
          : ["/api/registry"];
        let resolvedData: RegistryItem[] | null = null;

        for (const endpoint of endpointCandidates) {
          const candidateResponse = await fetch(`${endpoint}?${params.toString()}`, {
            headers: getAuthHeaders(),
            signal: controller.signal,
          });

          if (!candidateResponse.ok) {
            if (candidateResponse.status === 404) {
              continue;
            }
            throw new Error(`Request failed with status ${candidateResponse.status}`);
          }

          const contentType = candidateResponse.headers.get("content-type") ?? "";
          if (!contentType.includes("application/json")) {
            continue;
          }

          const candidateData = (await candidateResponse.json()) as unknown;
          if (!Array.isArray(candidateData)) {
            continue;
          }

          resolvedData = candidateData as RegistryItem[];
          break;
        }

        if (!resolvedData) {
          throw new Error("Registry endpoint not found.");
        }

        setItems(resolvedData);
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === "AbortError") {
          return;
        }

        setItems([]);
        setError("Failed to load registry items.");
      } finally {
        setLoading(false);
      }
    };

    void loadRegistryItems();

    return () => {
      controller.abort();
    };
  }, [page, search]);

  const handleSearchSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  const hasNextPage = items.length === PAGE_SIZE;

  return (
    <div>
      <PageMeta
        title="Registry Dashboard | Trading Engine"
        description="Registry dashboard page for Trading Engine."
      />
      <PageBreadcrumb pageTitle="Registry" />

      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="mb-4 flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
          <form className="flex w-full gap-2 md:max-w-xl" onSubmit={handleSearchSubmit}>
            <input
              type="text"
              value={searchInput}
              onChange={(event) => setSearchInput(event.target.value)}
              placeholder="Search teams..."
              className="h-11 w-full rounded-lg border border-gray-300 bg-transparent px-4 text-sm text-gray-800 outline-none focus:border-brand-500 dark:border-gray-700 dark:text-white/90"
            />
            <button
              type="submit"
              className="h-11 rounded-lg bg-brand-500 px-4 text-sm font-medium text-white hover:bg-brand-600"
            >
              Search
            </button>
          </form>
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
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Polymarket Home
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Polymarket Away
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Odds API Home
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Odds API Away
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Correlation
                </TableCell>
              </TableRow>
            </TableHeader>

            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <TableRow>
                  <TableCell className="px-5 py-6 text-sm text-gray-500 dark:text-gray-400" colSpan={5}>
                    Loading registry items...
                  </TableCell>
                </TableRow>
              ) : (
                items.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                      {item.polymarketHome}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                      {item.polymarketAway}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {item.oddsApiHome ?? "-"}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {item.oddsApiAway ?? "-"}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {item.correlationScore === null
                        ? "-"
                        : `${(item.correlationScore * 100).toFixed(1)}%`}
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

          <div className="text-sm text-gray-500 dark:text-gray-400">Page {page}</div>

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
    </div>
  );
}