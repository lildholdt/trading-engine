import { useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../ui/table";


// Define the TypeScript interface for the table rows
interface SportEvent {
  id: number; // Unique identifier for the sport event
  dateTime: Date; // Date and time of the sport event
  sport: string; // Type of sport (e.g., football, basketball)
  league: string; // League name (e.g., Premier League)
  team1: string; // Name of Team 1
  team2: string; // Name of Team 2
  market: string; // Market type
  marketDetail: number; // Market details as a decimal
  outcome1: number; // Outcome 1 value
  outcome2: number; // Outcome 2 value
  outcomeX: number; // Outcome X value (e.g., draw outcome)
  odds1: number; // Odds for outcome 1
  odds2: number; // Odds for outcome 2
  oddsX: number; // Odds for outcome X (e.g., draw odds)
}

// Define the table data using the interface

export default function SportEvents() {

  const [tableData, setTableData] = useState<SportEvent[]>([]);
  const [connection, setConnection] = useState<any>(null);

  // Establish SignalR connection
  useEffect(() => {
    const connectToSignalR = async () => {
      const hubConnection = new HubConnectionBuilder()
        .withUrl("http://localhost:5000/hubs/trading") // Replace with your SignalR server URL
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

      try {
        await hubConnection.start()
        console.log("SignalR connection established.");
        setConnection(hubConnection);

        // Listen for updates to the table data from the server
        hubConnection.on("SportEvent", (sportEvent: SportEvent) => {              
          setTableData((prevData) => {
          // Check if the ID of the incoming sportEvent already exists in the table data
          const exists = prevData.some((event) => event.id === sportEvent.id);
          
          // If the ID does not exist, add the sportEvent to the table data
          return exists ? prevData : [...prevData, sportEvent];
        });
        });
      } catch (err) {
        console.error("Error connecting to SignalR:", err);
      }
    };

    connectToSignalR();

    // Cleanup: Stop the SignalR connection when the component unmounts
    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, []);

  return (
    <div className="overflow-hidden rounded-2xl border border-gray-200 bg-white px-4 pb-3 pt-4 dark:border-gray-800 dark:bg-white/[0.03] sm:px-6">
      <div className="flex flex-col gap-2 mb-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">
            Sport Events
          </h3>
        </div>

        <div className="flex items-center gap-3">
          <button className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-theme-sm font-medium text-gray-700 shadow-theme-xs hover:bg-gray-50 hover:text-gray-800 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03] dark:hover:text-gray-200">
            <svg
              className="stroke-current fill-white dark:fill-gray-800"
              width="20"
              height="20"
              viewBox="0 0 20 20"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M2.29004 5.90393H17.7067"
                stroke=""
                strokeWidth="1.5"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M17.7075 14.0961H2.29085"
                stroke=""
                strokeWidth="1.5"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M12.0826 3.33331C13.5024 3.33331 14.6534 4.48431 14.6534 5.90414C14.6534 7.32398 13.5024 8.47498 12.0826 8.47498C10.6627 8.47498 9.51172 7.32398 9.51172 5.90415C9.51172 4.48432 10.6627 3.33331 12.0826 3.33331Z"
                fill=""
                stroke=""
                strokeWidth="1.5"
              />
              <path
                d="M7.91745 11.525C6.49762 11.525 5.34662 12.676 5.34662 14.0959C5.34661 15.5157 6.49762 16.6667 7.91745 16.6667C9.33728 16.6667 10.4883 15.5157 10.4883 14.0959C10.4883 12.676 9.33728 11.525 7.91745 11.525Z"
                fill=""
                stroke=""
                strokeWidth="1.5"
              />
            </svg>
            Filter
          </button>
          <button className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2.5 text-theme-sm font-medium text-gray-700 shadow-theme-xs hover:bg-gray-50 hover:text-gray-800 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03] dark:hover:text-gray-200">
            See all
          </button>
        </div>
      </div>
      <div className="max-w-full overflow-x-auto">
        <Table>
          {/* Table Header */}
          <TableHeader className="border-gray-100 dark:border-gray-800 border-y">
            <TableRow>
              <TableCell
                isHeader
                className="py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                Teams
              </TableCell>
              <TableCell
                isHeader
                className="py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                StartTime
              </TableCell>
              <TableCell
                isHeader
                className="py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                Sport
              </TableCell>
              <TableCell
                isHeader
                className="py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400"
              >
                League
              </TableCell>
            </TableRow>
          </TableHeader>

          {/* Table Body */}

          <TableBody className="divide-y divide-gray-100 dark:divide-gray-800">
            {tableData.map((sportEvent) => (
              <TableRow key={sportEvent.id} className="">
                <TableCell className="py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                  {sportEvent.team1} vs {sportEvent.team2}
                </TableCell>              
                <TableCell className="py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                  {sportEvent.dateTime.toLocaleString('en-GB', {
                      weekday: 'long',
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit',
                      second: '2-digit',
                    })}
                </TableCell>
                <TableCell className="py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                  {sportEvent.sport}
                </TableCell>
                <TableCell className="py-3 text-gray-500 text-theme-sm dark:text-gray-400">
                  {sportEvent.league}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
