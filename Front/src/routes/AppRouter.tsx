/* eslint-disable react-refresh/only-export-components */
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from "../App";
import LoginPage from "../pages/LoginPage";
import BrowseItems from "../pages/BrowseItems";
import ItemDetail from "../pages/ItemDetail";
import ManageUsers from "../pages/ManageUsers";
import ManageCategories from "../pages/ManageCategories";
import AdminDashboard from "../pages/AdminDashboard";
import { RedirectIfAuthed, RequireAuth, RequireRole } from "./guards";
import RegisterPage from '../pages/RegisterPage';
import ProfilePage from '../pages/ProfilePage';
import BrowseByCategory from '../pages/BrowseByCategory';
import AdminConvoCreate from '../pages/AdminConvoCreate';
import ChatPage from '../pages/ChatPage';
import SellItemForm from '../pages/SellItemForm';
import SellerDashboard from '../pages/SellerDashboard';
import ComingSoon from '../pages/ComingSoon';
import Recommendations from '../pages/Recommendations';

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