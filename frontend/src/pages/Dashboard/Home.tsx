import { FormEvent, Fragment, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
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
  series: string;
  startTime: string;
  isPaused?: boolean;
  isActive?: boolean;
  stoppedAtUtc?: string | null;
  odds: BookmakerOdds[];
};

type MatchViewMode = "live" | "history";

type BookmakerOdds = {
  name: string;
  home: number;
  away: number;
  draw: number;
  updatedAt: string;
};

type MatchSortKey = "home" | "away" | "series" | "startTime" | "bookmakers";
type SortDirection = "asc" | "desc";

const PAGE_SIZE = 20;
const API_BASE_URL =
  (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ?? "";

export default function Home() {
  const navigate = useNavigate();
  const [matches, setMatches] = useState<MatchItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [stoppingId, setStoppingId] = useState<string | null>(null);
  const [togglingPauseId, setTogglingPauseId] = useState<string | null>(null);
  const [expandedMatchIds, setExpandedMatchIds] = useState<Set<string>>(new Set());

  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [viewMode, setViewMode] = useState<MatchViewMode>("live");
  const [sortKey, setSortKey] = useState<MatchSortKey>("startTime");
  const [sortDirection, setSortDirection] = useState<SortDirection>("asc");

  useEffect(() => {
    const controller = new AbortController();

    const loadMatches = async () => {
      setLoading(true);
      setError(null);

      try {
        const params = new URLSearchParams({
          page: String(page),
          pageSize: String(PAGE_SIZE),
          sortByStartTime: "asc",
        });

        if (search.trim()) {
          params.set("search", search.trim());
        }

        const endpointPath = viewMode === "live" ? "/api/matches/live" : "/api/matches/history";
        const endpointCandidates = API_BASE_URL
          ? [`${API_BASE_URL}${endpointPath}`, endpointPath]
          : [endpointPath];

        let resolvedData: MatchItem[] | null = null;

        for (const endpoint of endpointCandidates) {
          const response = await fetch(`${endpoint}?${params.toString()}`, {
            headers: getAuthHeaders(),
            signal: controller.signal,
          });

          if (!response.ok) {
            if (response.status === 404) {
              continue;
            }
            throw new Error(`Request failed with status ${response.status}`);
          }

          const contentType = response.headers.get("content-type") ?? "";
          if (!contentType.includes("application/json")) {
            continue;
          }

          const data = (await response.json()) as unknown;
          if (!Array.isArray(data)) {
            continue;
          }

          resolvedData = data as MatchItem[];
          break;
        }

        if (!resolvedData) {
          throw new Error("Matches endpoint not found.");
        }

        setMatches(resolvedData);
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === "AbortError") {
          return;
        }

        setMatches([]);
        setError("Failed to load matches.");
      } finally {
        setLoading(false);
      }
    };

    void loadMatches();

    return () => {
      controller.abort();
    };
  }, [page, search, viewMode]);

  const handleSearchSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  const handleViewModeChange = (nextMode: MatchViewMode) => {
    if (viewMode === nextMode) {
      return;
    }

    setPage(1);
    setExpandedMatchIds(new Set());
    setViewMode(nextMode);
  };

  const handleStopMatch = async (id: string) => {
    setStoppingId(id);
    setError(null);

    const previousMatches = matches;
    setMatches((prev) => prev.filter((match) => match.id !== id));
    setExpandedMatchIds((prev) => {
      const next = new Set(prev);
      next.delete(id);
      return next;
    });

    try {
      const endpointCandidates = API_BASE_URL
        ? [`${API_BASE_URL}/api/matches/live/${id}`, `/api/matches/live/${id}`]
        : [`/api/matches/live/${id}`];

      let stopped = false;
      for (const endpoint of endpointCandidates) {
        const response = await fetch(endpoint, { method: "DELETE", headers: getAuthHeaders() });
        if (response.ok) {
          stopped = true;
          break;
        }
        if (response.status !== 404) {
          throw new Error(`Request failed with status ${response.status}`);
        }
      }

      if (!stopped) {
        throw new Error("Stop match endpoint not found.");
      }
    } catch {
      setMatches(previousMatches);
      setError("Failed to stop match.");
    } finally {
      setStoppingId(null);
    }
  };

  const handlePauseToggle = async (id: string, nextPausedState: boolean) => {
    setTogglingPauseId(id);
    setError(null);

    const previousMatches = matches;
    setMatches((prev) =>
      prev.map((match) => (match.id === id ? { ...match, isPaused: nextPausedState } : match))
    );

    try {
      const actionPath = nextPausedState ? "pause" : "resume";
      const endpointCandidates = API_BASE_URL
        ? [`${API_BASE_URL}/api/matches/live/${id}/${actionPath}`, `/api/matches/live/${id}/${actionPath}`]
        : [`/api/matches/live/${id}/${actionPath}`];

      let updated = false;
      for (const endpoint of endpointCandidates) {
        const response = await fetch(endpoint, { method: "POST", headers: getAuthHeaders() });
        if (response.ok) {
          updated = true;
          break;
        }

        if (response.status !== 404) {
          throw new Error(`Request failed with status ${response.status}`);
        }
      }

      if (!updated) {
        throw new Error("Pause/resume endpoint not found.");
      }
    } catch {
      setMatches(previousMatches);
      setError("Failed to update match pause state.");
    } finally {
      setTogglingPauseId(null);
    }
  };

  const hasNextPage = matches.length === PAGE_SIZE;

  const sortedMatches = useMemo(() => {
    const directionMultiplier = sortDirection === "asc" ? 1 : -1;

    return [...matches].sort((left, right) => {
      if (sortKey === "bookmakers") {
        return ((left.odds?.length ?? 0) - (right.odds?.length ?? 0)) * directionMultiplier;
      }

      if (sortKey === "startTime") {
        return (
          (new Date(left.startTime).getTime() - new Date(right.startTime).getTime()) *
          directionMultiplier
        );
      }

      return left[sortKey].localeCompare(right[sortKey]) * directionMultiplier;
    });
  }, [matches, sortDirection, sortKey]);

  const handleSort = (nextSortKey: MatchSortKey) => {
    if (sortKey === nextSortKey) {
      setSortDirection((prev) => (prev === "asc" ? "desc" : "asc"));
      return;
    }

    setSortKey(nextSortKey);
    setSortDirection(nextSortKey === "startTime" ? "asc" : "asc");
  };

  const getSortIndicator = (columnKey: MatchSortKey) => {
    if (sortKey !== columnKey) {
      return "";
    }

    return sortDirection === "asc" ? " \u2191" : " \u2193";
  };

  const toggleExpandedRow = (matchId: string) => {
    setExpandedMatchIds((prev) => {
      const next = new Set(prev);
      if (next.has(matchId)) {
        next.delete(matchId);
      } else {
        next.add(matchId);
      }
      return next;
    });
  };

  return (
    <>
      <PageMeta
        title="Matches | Trading Engine"
        description="Matches page for Trading Engine."
      />
      <PageBreadcrumb pageTitle="Matches" />

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

          <div className="inline-flex rounded-lg border border-gray-300 p-1 dark:border-gray-700">
            <button
              type="button"
              onClick={() => handleViewModeChange("live")}
              className={`rounded-md px-3 py-1.5 text-sm ${
                viewMode === "live"
                  ? "bg-brand-500 text-white"
                  : "text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/5"
              }`}
            >
              Live
            </button>
            <button
              type="button"
              onClick={() => handleViewModeChange("history")}
              className={`rounded-md px-3 py-1.5 text-sm ${
                viewMode === "history"
                  ? "bg-brand-500 text-white"
                  : "text-gray-600 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/5"
              }`}
            >
              History
            </button>
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
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Details
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  <button
                    type="button"
                    onClick={() => handleSort("home")}
                    className="inline-flex items-center gap-1 hover:text-gray-700 dark:hover:text-gray-200"
                  >
                    Home{getSortIndicator("home")}
                  </button>
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  <button
                    type="button"
                    onClick={() => handleSort("away")}
                    className="inline-flex items-center gap-1 hover:text-gray-700 dark:hover:text-gray-200"
                  >
                    Away{getSortIndicator("away")}
                  </button>
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  <button
                    type="button"
                    onClick={() => handleSort("series")}
                    className="inline-flex items-center gap-1 hover:text-gray-700 dark:hover:text-gray-200"
                  >
                    Series{getSortIndicator("series")}
                  </button>
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  <button
                    type="button"
                    onClick={() => handleSort("startTime")}
                    className="inline-flex items-center gap-1 hover:text-gray-700 dark:hover:text-gray-200"
                  >
                    Start Time{getSortIndicator("startTime")}
                  </button>
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  <button
                    type="button"
                    onClick={() => handleSort("bookmakers")}
                    className="inline-flex items-center gap-1 hover:text-gray-700 dark:hover:text-gray-200"
                  >
                    Bookmakers{getSortIndicator("bookmakers")}
                  </button>
                </TableCell>
                {viewMode === "live" && (
                  <TableCell
                    isHeader
                    className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                  >
                    Status
                  </TableCell>
                )}
                {viewMode === "live" && (
                  <TableCell
                    isHeader
                    className="w-40 px-5 py-3 text-end text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                  >
                    Actions
                  </TableCell>
                )}
              </TableRow>
            </TableHeader>

            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <TableRow>
                  <TableCell
                    className="px-5 py-6 text-sm text-gray-500 dark:text-gray-400"
                    colSpan={viewMode === "live" ? 8 : 6}
                  >
                    Loading matches...
                  </TableCell>
                </TableRow>
              ) : (
                sortedMatches.map((match) => {
                  const isExpanded = expandedMatchIds.has(match.id);

                  return (
                    <Fragment key={match.id}>
                      <TableRow
                        className="cursor-pointer hover:bg-gray-50 dark:hover:bg-white/[0.02]"
                        onClick={() => navigate(`/matches/${match.id}`, { state: { match, viewMode } })}
                      >
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          <button
                            type="button"
                            aria-label={isExpanded ? "Collapse details" : "Expand details"}
                            onClick={(event) => {
                              event.stopPropagation();
                              toggleExpandedRow(match.id);
                            }}
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
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                          {match.home}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                          {match.away}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                          {match.series}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {formatEuropeanDateTime(match.startTime)}
                        </TableCell>
                        <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                          {match.odds?.length ?? 0}
                        </TableCell>
                        {viewMode === "live" && (
                          <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-500 dark:text-gray-400">
                            <span
                              className={`inline-flex rounded-full px-2 py-1 text-xs font-medium ${
                                match.isPaused
                                  ? "bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300"
                                  : "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300"
                              }`}
                            >
                              {match.isPaused ? "Paused" : "Live"}
                            </span>
                          </TableCell>
                        )}
                        {viewMode === "live" && (
                          <TableCell className="w-40 px-5 py-3 text-end text-theme-sm text-gray-500 dark:text-gray-400">
                            <div className="inline-flex items-center gap-2">
                              <button
                                type="button"
                                onClick={(event) => {
                                  event.stopPropagation();
                                  void handlePauseToggle(match.id, !Boolean(match.isPaused));
                                }}
                                disabled={togglingPauseId === match.id}
                                className="rounded-lg border border-amber-300 px-3 py-1.5 text-xs font-medium text-amber-700 hover:bg-amber-50 disabled:cursor-not-allowed disabled:opacity-60 dark:border-amber-800 dark:text-amber-300 dark:hover:bg-amber-900/20"
                              >
                                {togglingPauseId === match.id
                                  ? "Updating..."
                                  : match.isPaused
                                    ? "Resume"
                                    : "Pause"}
                              </button>
                              <button
                                type="button"
                                onClick={(event) => {
                                  event.stopPropagation();
                                  void handleStopMatch(match.id);
                                }}
                                disabled={stoppingId === match.id}
                                className="rounded-lg border border-red-300 px-3 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50 disabled:cursor-not-allowed disabled:opacity-60 dark:border-red-800 dark:text-red-300 dark:hover:bg-red-900/20"
                              >
                                {stoppingId === match.id ? "Stopping..." : "Stop"}
                              </button>
                            </div>
                          </TableCell>
                        )}
                      </TableRow>

                      {isExpanded && (
                        <TableRow>
                          <TableCell
                            className="bg-gray-50/60 px-5 py-4 dark:bg-white/[0.02]"
                            colSpan={viewMode === "live" ? 8 : 6}
                          >
                            {match.odds.length === 0 ? (
                              <p className="text-sm text-gray-500 dark:text-gray-400">
                                No bookmaker details available for this match.
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
                                        Home
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Draw
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Away
                                      </th>
                                      <th className="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-400">
                                        Updated
                                      </th>
                                    </tr>
                                  </thead>
                                  <tbody>
                                    {match.odds.map((oddsItem, index) => (
                                      <tr
                                        key={`${oddsItem.name}-${oddsItem.updatedAt}-${index}`}
                                        className="border-t border-gray-100 dark:border-white/[0.05]"
                                      >
                                        <td className="px-3 py-2 text-sm text-gray-700 dark:text-gray-300">
                                          {oddsItem.name}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {oddsItem.home}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {oddsItem.draw}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {oddsItem.away}
                                        </td>
                                        <td className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                                          {formatEuropeanDateTime(oddsItem.updatedAt)}
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
    </>
  );
}
