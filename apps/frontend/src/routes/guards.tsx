import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../store/authstore';
import type { Role } from '../api/types';

function Checking() {
  return <div className="p-6 text-sm text-gray-600">Checking session…</div>;
}

export const RequireAuth = () => {
  const { status } = useAuth(); 
  const location = useLocation();

  if (status === 'idle' || status === 'checking') return <Checking />;
  if (status !== 'authed') {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }
  return <Outlet />;
};

export const RequireRole = ({ roles }: { roles: Role[] }) => {
  const { status, user } = useAuth();
  const location = useLocation();

  if (status === 'idle' || status === 'checking') return <Checking />;
  if (status !== 'authed' || !user) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }
  if (!user.role || !roles.includes(user.role)) {

    return <Navigate to="/" replace />;
  }
  return <Outlet />;
};

export const RedirectIfAuthed = () => {
  const { status } = useAuth();
  if (status === "idle" || status === "checking") return <Checking />;
  if (status === "authed") return <Navigate to="/" replace />;
  return <Outlet />; 
};

