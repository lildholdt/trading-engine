import { useEffect, useState } from "react";
import { useLocation, useParams } from "react-router";
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
  startTime: string;
};

type OrderItem = {
  id: string;
  bookmaker: string;
  snapshotTime: string;
  hoursBefore: number;
  oddsHome: number;
  oddsDraw: number;
  oddsAway: number;
  trueOddsHome: number;
  trueOddsDraw: number;
  trueOddsAway: number;
  trueOddsAverageHome: number;
  trueOddsAverageDraw: number;
  trueOddsAverageAway: number;
  polymarketOutcomeHome: number;
  polymarketOutcomeDraw: number;
  polymarketOutcomeAway: number;
};

const API_BASE_URL =
  (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ?? "";

export default function MatchDetails() {
  const { id } = useParams<{ id: string }>();
  const location = useLocation();

  const [match, setMatch] = useState<MatchItem | null>((location.state as { match?: MatchItem } | null)?.match ?? null);
  const [orders, setOrders] = useState<OrderItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      return;
    }

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

    const loadDetails = async () => {
      setLoading(true);
      setError(null);

      try {
        if (!match) {
          const matchEndpoints = API_BASE_URL
            ? [`${API_BASE_URL}/api/matches?page=1&pageSize=500`, "/api/matches?page=1&pageSize=500"]
            : ["/api/matches?page=1&pageSize=500"];

          for (const endpoint of matchEndpoints) {
            const data = await fetchJsonArray(endpoint);
            if (!data) {
              continue;
            }

            const found = (data as MatchItem[]).find((item) => item.id === id) ?? null;
            if (found) {
              setMatch(found);
            }
            break;
          }
        }

        const orderEndpoints = API_BASE_URL
          ? [`${API_BASE_URL}/api/matches/${id}/orders`, `/api/matches/${id}/orders`]
          : [`/api/matches/${id}/orders`];

        let resolvedOrders: OrderItem[] | null = null;
        for (const endpoint of orderEndpoints) {
          const data = await fetchJsonArray(endpoint);
          if (!data) {
            continue;
          }

          resolvedOrders = data as OrderItem[];
          break;
        }

        if (!resolvedOrders) {
          throw new Error("Orders endpoint not found.");
        }

        setOrders(resolvedOrders);
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === "AbortError") {
          return;
        }

        setOrders([]);
        setError("Failed to load match details.");
      } finally {
        setLoading(false);
      }
    };

    void loadDetails();

    return () => {
      controller.abort();
    };
  }, [id, match]);

  return (
    <>
      <PageMeta title="Match Details | Trading Engine" description="Match details page for Trading Engine." />
      <PageBreadcrumb pageTitle="Match Details" />

      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="mb-5 grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <div className="rounded-xl border border-gray-200 p-3 dark:border-gray-700">
            <p className="text-xs text-gray-500 dark:text-gray-400">Match</p>
            <p className="mt-1 text-sm font-medium text-gray-800 dark:text-gray-100">
              {match ? `${match.home} vs ${match.away}` : "-"}
            </p>
          </div>
          <div className="rounded-xl border border-gray-200 p-3 dark:border-gray-700">
            <p className="text-xs text-gray-500 dark:text-gray-400">Start Time</p>
            <p className="mt-1 text-sm font-medium text-gray-800 dark:text-gray-100">
              {match?.startTime ? new Date(match.startTime).toLocaleString() : "-"}
            </p>
          </div>
          <div className="rounded-xl border border-gray-200 p-3 dark:border-gray-700">
            <p className="text-xs text-gray-500 dark:text-gray-400">Match Id</p>
            <p className="mt-1 text-sm font-medium text-gray-800 dark:text-gray-100 break-all">{id ?? "-"}</p>
          </div>
          <div className="rounded-xl border border-gray-200 p-3 dark:border-gray-700">
            <p className="text-xs text-gray-500 dark:text-gray-400">Orders</p>
            <p className="mt-1 text-sm font-medium text-gray-800 dark:text-gray-100">{orders.length}</p>
          </div>
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
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  True Odds (H/D/A)
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
                  <TableRow key={`${order.id}-${index}`}>
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
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                      {order.trueOddsHome} / {order.trueOddsDraw} / {order.trueOddsAway}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </div>
    </>
  );
}
