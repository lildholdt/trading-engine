import { SidebarProvider, useSidebar } from "../context/SidebarContext";
import { Outlet, useNavigate } from "react-router";
import { useEffect, useRef } from "react";
import AppHeader from "./AppHeader";
import AppSidebar from "./AppSidebar";
import { getAuthToken } from "../utils/auth";

const LayoutContent: React.FC = () => {
  const navigate = useNavigate();
  const { isExpanded, isHovered, isMobileOpen, toggleMobileSidebar } = useSidebar();
  const sidebarRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!getAuthToken()) {
      navigate("/login", { replace: true });
    }
  }, [navigate]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const target = event.target as HTMLElement;
      if (isMobileOpen && sidebarRef.current && !sidebarRef.current.contains(target)) {
        // Don't close if clicking the toggle button
        if (!target.closest('[data-sidebar-toggle]')) {
          toggleMobileSidebar();
        }
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [isMobileOpen, toggleMobileSidebar]);

  return (
    <div className="min-h-screen xl:flex">
      <div ref={sidebarRef}>
        <AppSidebar />
      </div>
      <div
        className={`flex-1 transition-all duration-300 ease-in-out ${
          isExpanded || isHovered ? "lg:ml-[290px]" : "lg:ml-[90px]"
        } ${isMobileOpen ? "ml-0" : ""}`}
      >
        <AppHeader />
        <div className="p-4 mx-auto max-w-(--breakpoint-2xl) md:p-6">
          <Outlet />
        </div>
      </div>
    </div>
  );
};

const AppLayout: React.FC = () => {
  return (
    <SidebarProvider>
      <LayoutContent />
    </SidebarProvider>
  );
};

export default AppLayout;
