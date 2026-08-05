import { Outlet, Link } from 'react-router-dom';
import { useAuth } from './store/authstore';
import { useEffect } from 'react';


export default function App(){
    const { user, logout, hydrate } = useAuth();
    
    useEffect(() => {
        void hydrate();
    }, [hydrate]);

    return (
    <div>
        <nav className="flex items-center justify-between p-4 border-b">
            <Link to="/" className="font-semibold">DefNotEbay</Link>
            {user?.role !== 'Admin' && user?.role !== 'Seller' && <Link to="/browse-categories" className="underline">Browse By Category</Link>}
            <div className="flex items-center gap-3">
            {user ? (
            <>
                {user.role === 'Seller' && <Link to="/seller-dashboard" className="underline">Seller Dashboard</Link>}
                {user.role === 'Admin' && <Link to="/admin" className="underline">Admin</Link>}
                {user.role !== 'Admin' && <Link to="/inbox" className="underline">Inbox</Link>}
                {user.role == 'Buyer' && <Link to="/for-you" className="underline">For You</Link>}
                <Link to = "/me"><span className="opacity-70">{user.name}</span></Link>
                <button onClick={logout} className="border px-3 py-1 rounded">Logout</button>
            </>
            ) : (
            <>
                <Link to="/login" className="underline">Login</Link>
                <Link to="/register" className="underline">Register</Link>
                <Link to="/login" className="underline">Sell</Link>
            </>
            )
            } 
            </div>
        </nav>
        <Outlet/>
    </div>
    );
}