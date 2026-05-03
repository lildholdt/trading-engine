import { Fragment, useEffect, useState } from "react";
import { useLocation, useParams } from "react-router";
import Chart from "react-apexcharts";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../../components/ui/table";
import { formatEuropeanDateTime } from "../../utils/dateTime";
import { getAuthHeaders } from "../../utils/auth";

type MatchItem = {
  id: string;
  home: string;
  away: string;
  series?: string;
  startTime: string;
  isActive?: boolean;
};

type MatchViewMode = "live" | "history";

type OrderBookmaker = {
  name: string;
  oddsHome: number;
  oddsDraw: number;
  oddsAway: number;
  trueOddsHome: number;
  trueOddsDraw: number;
  trueOddsAway: number;
};

type OrderItem = {
  id: string;
  snapshotTime: string;
  hoursBefore: number;
  bookmakers: OrderBookmaker[];
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
  const locationState = (location.state as { match?: MatchItem; viewMode?: MatchViewMode } | null) ?? null;

  const [match, setMatch] = useState<MatchItem | null>(locationState?.match ?? null);
  const viewMode: MatchViewMode = locationState?.viewMode ?? "live";
  const [orders, setOrders] = useState<OrderItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [expandedOrderIds, setExpandedOrderIds] = useState<Set<string>>(new Set());
  const [chartOptions, setChartOptions] = useState<any>(null);
  const [chartSeries, setChartSeries] = useState<any>(null);

  useEffect(() => {
    if (!id) {
      return;
    }

    const controller = new AbortController();

    const fetchJsonArray = async (endpoint: string): Promise<unknown[] | null> => {
      const response = await fetch(endpoint, {
        headers: getAuthHeaders(),
        signal: controller.signal,
      });
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

    const fetchJsonObject = async (endpoint: string): Promise<Record<string, unknown> | null> => {
      const response = await fetch(endpoint, {
        headers: getAuthHeaders(),
        signal: controller.signal,
      });
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
      if (typeof data !== "object" || data === null || Array.isArray(data)) {
        return null;
      }

      return data as Record<string, unknown>;
    };

    const loadDetails = async () => {
      setLoading(true);
      setError(null);

      try {
        if (!match) {
          const detailsEndpoint =
            viewMode === "live" ? `/api/matches/live/${id}` : `/api/matches/history/${id}`;
          const matchEndpoints = API_BASE_URL
            ? [`${API_BASE_URL}${detailsEndpoint}`, detailsEndpoint]
            : [detailsEndpoint];

          for (const endpoint of matchEndpoints) {
            const data = await fetchJsonObject(endpoint);
            if (!data) {
              continue;
            }

            setMatch(data as unknown as MatchItem);
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

        const sorted = [...resolvedOrders].sort(
          (a, b) => new Date(b.snapshotTime).getTime() - new Date(a.snapshotTime).getTime()
        );
        setOrders(sorted);
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
  }, [id, match, viewMode]);

  useEffect(() => {
    if (orders.length === 0) {
      return;
    }

    // Sort orders by snapshot time
    const sortedOrders = [...orders].sort((a, b) => 
      new Date(a.snapshotTime).getTime() - new Date(b.snapshotTime).getTime()
    );

    // Create accumulated data
    const timeLabels: string[] = [];
    const accumulatedCounts: number[] = [];
    let cumulative = 0;

    sortedOrders.forEach((order) => {
      const time = formatEuropeanDateTime(order.snapshotTime);
      timeLabels.push(time);
      cumulative += 1;
      accumulatedCounts.push(cumulative);
    });

    setChartOptions({
      chart: {
        type: "line",
        toolbar: {
          show: false,
        },
      },
      stroke: {
        curve: "straight",
        width: 2,
      },
      markers: {
        size: 5,
        colors: "#3b82f6",
        strokeWidth: 0,
        hover: {
          size: 7,
        },
      },
      dataLabels: {
        enabled: false,
      },
      colors: ["#3b82f6"],
      xaxis: {
        type: "category",
        categories: timeLabels,
        axisBorder: {
          show: false,
        },
        axisTicks: {
          show: false,
        },
        labels: {
          show: true,
          style: {
            fontSize: "12px",
          },
        },
      },
      yaxis: {
        title: {
          text: "Accumulated Orders",
        },
      },
      grid: {
        show: true,
        borderColor: "#e5e7eb",
        strokeDashArray: 0,
        position: "back",
      },
      tooltip: {
        theme: "light",
        y: {
          formatter: (value: number) => `${value} orders`,
        },
      },
    });

    setChartSeries([
      {
        name: "Accumulated Orders",
        data: accumulatedCounts,
      },
    ]);
  }, [orders]);

  const toggleExpandedOrder = (orderKey: string) => {
    setExpandedOrderIds((prev) => {
      const next = new Set(prev);
      if (next.has(orderKey)) {
        next.delete(orderKey);
      } else {
        next.add(orderKey);
      }
      return next;
    });
  };

  return (
    <>
      <PageMeta title="Match Details | Trading Engine" description="Match details page for Trading Engine." />
      <PageBreadcrumb pageTitle="Match Details" />

      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="mb-5 grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <div className="rounded-xl border border-gray-200 p-3 dark:border-gray-700">
            <p className="text-xs text-gray-500 dark:text-gray-400">Match</p>
            <p className="mt-1 text-sm font-medium text-gray-800 dark:text-gray-100">
              {match ? (
                <>
                  {match.home}<span className="mx-3 text-blue-500">vs</span>{match.away}
                </>
              ) : (
                "-"
              )}
            </p>
          </div>
          <div className="rounded-xl border border-gray-200 p-3 dark:border-gray-700">
            <p className="text-xs text-gray-500 dark:text-gray-400">Start Time</p>
            <p className="mt-1 text-sm font-medium text-gray-800 dark:text-gray-100">
              {match?.startTime ? formatEuropeanDateTime(match.startTime) : "-"}
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

        {chartOptions && chartSeries && (
          <div className="mb-5 rounded-xl border border-gray-200 bg-gray-50 p-4 dark:border-gray-700 dark:bg-white/[0.02]">
            <h3 className="mb-4 text-sm font-medium text-gray-700 dark:text-gray-300">Orders Timeline</h3>
            <Chart options={chartOptions} series={chartSeries} type="line" height={300} />
          </div>
        )}

        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
              <TableRow>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Details
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Snapshot Time
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Hours Before
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Bookmakers
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  True Odds Avg (H/D/A)
                </TableCell>
                <TableCell isHeader className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400">
                  Polymarket (H/D/A)
                </TableCell>
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <TableRow>
                  <TableCell className="px-5 py-6 text-sm text-gray-500 dark:text-gray-400" colSpan={6}>
                    Loading orders...
                  </TableCell>
                </TableRow>
              ) : orders.length === 0 ? (
                <TableRow>
                  <TableCell className="px-5 py-6 text-sm text-gray-500 dark:text-gray-400" colSpan={6}>
                    No orders available for this match.
                  </TableCell>
                </TableRow>
              ) : (
                orders.map((order, index) => {
                  const orderKey = `${order.snapshotTime}-${index}`;
                  const isExpanded = expandedOrderIds.has(orderKey);

                  return (
                    <Fragment key={orderKey}>
                      <TableRow className="hover:bg-gray-50 dark:hover:bg-white/[0.02]">
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          <button
                            type="button"
                            aria-label={isExpanded ? "Collapse details" : "Expand details"}
                            onClick={() => toggleExpandedOrder(orderKey)}
                            className="inline-flex h-7 w-7 items-center justify-center rounded-md border border-gray-300 text-gray-600 hover:bg-gray-100 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5"
                          >
                            <svg
                              className={`h-4 w-4 transition-transform ${isExpanded ? "rotate-180" : ""}`}
                              viewBox="0 0 20 20"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                d="M5 8L10 13L15 8"
                                stroke="currentColor"
                                strokeWidth="1.8"
                                strokeLinecap="round"
                                strokeLinejoin="round"
                              />
                            </svg>
                          </button>
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {formatEuropeanDateTime(order.snapshotTime)}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {order.hoursBefore}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {order.bookmakers.length}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {order.trueOddsAverageHome} / {order.trueOddsAverageDraw} / {order.trueOddsAverageAway}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {order.polymarketOutcomeHome} / {order.polymarketOutcomeDraw} / {order.polymarketOutcomeAway}
                        </TableCell>
                      </TableRow>

                      {isExpanded && (
                        <TableRow>
                          <TableCell className="bg-gray-50/60 px-5 py-4 dark:bg-white/[0.02]" colSpan={6}>
                            {order.bookmakers.length === 0 ? (
                              <p className="text-sm text-gray-500 dark:text-gray-400">
                                No bookmaker details available for this snapshot.
                              </p>
                            ) : (
                              <div className="overflow-x-auto">
                                <table className="min-w-full">
                                  <thead>
                                    <tr>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Bookmaker
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Odds Home
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Odds Draw
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Odds Away
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        True Home
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        True Draw
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        True Away
                                      </th>
                                    </tr>
                                  </thead>
                                  <tbody>
                                    {order.bookmakers.map((bookmaker, bookmakerIndex) => (
                                      <tr
                                        key={`${bookmaker.name}-${bookmakerIndex}`}
                                        className="border-t border-gray-100 dark:border-white/[0.05]"
                                      >
                                        <td className="px-3 py-2 text-sm text-gray-700 dark:text-gray-300">
                                          {bookmaker.name}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {bookmaker.oddsHome}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {bookmaker.oddsDraw}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {bookmaker.oddsAway}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {bookmaker.trueOddsHome}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {bookmaker.trueOddsDraw}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {bookmaker.trueOddsAway}
                                        </td>
                                      </tr>
                                    ))}
                                  </tbody>
                                </table>
                              </div>
                            )}
                          </TableCell>
                        </TableRow>
                      )}
                    </Fragment>
                  );
                })
              )}
            </TableBody>
          </Table>
        </div>
      </div>
    </>
  );
}
