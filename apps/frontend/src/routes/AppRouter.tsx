/* eslint-disable react-refresh/only-export-components */
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from "../App";
import LoginPage from "../pages/auth/LoginPage";
import BrowseItems from "../pages/browse/BrowseItems";
import ItemDetail from "../pages/browse/ItemDetail";
import ManageUsers from "../pages/admin/ManageUsers";
import ManageCategories from "../pages/admin/ManageCategories";
import AdminDashboard from "../pages/admin/AdminDashboard";
import { RedirectIfAuthed, RequireAuth, RequireRole } from "./guards";
import RegisterPage from '../pages/auth/RegisterPage';
import ProfilePage from '../pages/misc/ProfilePage';
import BrowseByCategory from '../pages/browse/BrowseByCategory';
import AdminConvoCreate from '../pages/admin/AdminConvoCreate';
import ChatPage from '../pages/misc/ChatPage';
import SellItemForm from '../pages/seller/SellItemForm';
import SellerDashboard from '../pages/seller/SellerDashboard';
import ComingSoon from '../pages/misc/ComingSoon';
import Recommendations from '../pages/browse/Recommendations';

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    errorElement: <ComingSoon />,
    children: [
      { index: true, element: <BrowseItems /> }, //public
      { path: "browse", element: <BrowseItems /> },
      { path: "items/:id", element: <ItemDetail /> },
      { path: "browse-categories", element: <BrowseByCategory /> },

      {
        element: <RedirectIfAuthed />, //only visitors
        children: [
          { path: "login", element: <LoginPage /> },
          { path: "register", element: <RegisterPage /> },
        ],
      },

    {
        element: <RequireAuth />,  //only authed users
        children: [
          { path: "me", element: <ProfilePage /> }
        ],
      },

      {
        element: <RequireRole roles={["Admin"]} />, 
        children: [
          { path: "admin", element: <AdminDashboard /> },
          { path: "admin/users", element: <ManageUsers /> },
          { path: "admin/categories", element: <ManageCategories /> },
          { path: "admin/create-chat", element: <AdminConvoCreate /> },
        ],
      },

      {
        element: <RequireRole roles={["Seller", "Buyer"]} />, 
        children: [
          { path: "inbox", element: <ChatPage /> },
        ],
      },
      
      {
        element: <RequireRole roles={["Seller"]} />, 
        children: [
          { path: "sell", element: <SellItemForm /> },
          { path: "seller-dashboard", element: <SellerDashboard /> },
        ],
      },
      
      {
        element: <RequireRole roles={["Buyer"]} />, 
        children: [
          { path: "for-you", element: <Recommendations /> },
        ],
      }
    ],
  },
]);

export default function AppRouter() { return <RouterProvider router={router}/>; }