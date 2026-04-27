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

type RegistryConfigurationItem = {
  id: number;
  name: string;
  active: boolean;
};

const API_BASE_URL =
  (import.meta.env.VITE_API_BASE_URL as string | undefined)?.replace(/\/$/, "") ?? "";

export default function RegistryConfiguration() {
  const [items, setItems] = useState<RegistryConfigurationItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [updatingId, setUpdatingId] = useState<number | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    const loadConfiguration = async () => {
      setLoading(true);
      setError(null);

      try {
        const endpointCandidates = API_BASE_URL
          ? [`${API_BASE_URL}/api/registry/configuration`, "/api/registry/configuration"]
          : ["/api/registry/configuration"];

        let resolvedData: RegistryConfigurationItem[] | null = null;

        for (const endpoint of endpointCandidates) {
          const response = await fetch(endpoint, {
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

          resolvedData = data as RegistryConfigurationItem[];
          break;
        }

        if (!resolvedData) {
          throw new Error("Configuration endpoint not found.");
        }

        setItems(resolvedData);
      } catch (requestError) {
        if (requestError instanceof DOMException && requestError.name === "AbortError") {
          return;
        }

        setItems([]);
        setError("Failed to load configuration.");
      } finally {
        setLoading(false);
      }
    };

    void loadConfiguration();

    return () => {
      controller.abort();
    };
  }, []);

  const updateConfiguration = async (id: number, nextState: boolean) => {
    setUpdatingId(id);
    setError(null);

    const previousItems = items;
    setItems((prev) => prev.map((item) => (item.id === id ? { ...item, active: nextState } : item)));

    try {
      const endpointCandidates = API_BASE_URL
        ? [
            `${API_BASE_URL}/api/registry/configuration/${id}?state=${nextState}`,
            `/api/registry/configuration/${id}?state=${nextState}`,
          ]
        : [`/api/registry/configuration/${id}?state=${nextState}`];

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
        throw new Error("Configuration update endpoint not found.");
      }
    } catch {
      setItems(previousItems);
      setError("Failed to update configuration.");
    } finally {
      setUpdatingId(null);
    }
  };

  return (
    <div>
      <PageMeta
        title="Registry Configuration | Trading Engine"
        description="Registry configuration page for Trading Engine."
      />
      <PageBreadcrumb pageTitle="Registry Configuration" />

      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
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
                  Name
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-end text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  Active
                </TableCell>
              </TableRow>
            </TableHeader>

            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {loading ? (
                <TableRow>
                  <TableCell className="px-5 py-6 text-sm text-gray-500 dark:text-gray-400" colSpan={2}>
                    Loading configuration...
                  </TableCell>
                </TableRow>
              ) : (
                items.map((item) => (
                  <TableRow key={item.id}>
                    <TableCell className="px-5 py-3 text-start text-theme-sm text-gray-700 dark:text-gray-300">
                      {item.name}
                    </TableCell>
                    <TableCell className="px-5 py-3 text-theme-sm text-gray-700 dark:text-gray-300">
                      <div className="flex justify-end">
                        <label className="inline-flex cursor-pointer items-center">
                        <input
                          type="checkbox"
                          className="sr-only"
                          checked={item.active}
                          disabled={updatingId === item.id}
                          onChange={() => updateConfiguration(item.id, !item.active)}
                        />
                        <span
                          className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
                            item.active ? "bg-brand-500" : "bg-gray-300 dark:bg-gray-700"
                          } ${updatingId === item.id ? "opacity-60" : ""}`}
                        >
                          <span
                            className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                              item.active ? "translate-x-6" : "translate-x-1"
                            }`}
                          />
                        </span>
                        </label>
                      </div>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      </div>
    </div>
  );
}