import "./App.css";
import Login from "./pages/Login.tsx";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import "semantic-ui-css/semantic.min.css";
import Accounts from "./components/Accounts.tsx";
import { AuthProvider } from "./context/AuthContext.tsx";
import "react-toastify/dist/ReactToastify.min.css";
import Logout from "./components/Logout.tsx";
// import Login from './components/Login.tsx';

const PrivateRoute = ({ children }: { children: JSX.Element }) => {
  //   const { user } = useAuth();
  const currentAuth = localStorage.getItem("accessToken");
  //   console.log("CurrentUser", user);
  if (!currentAuth) return <Navigate to="/login" replace />;
  return children;
};

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <ToastContainer position="top-center" style={{ zIndex: 5000 }} />
        <Routes>
          <Route path="login" element={<Login />} />
          <Route
            path="/"
            element={
              <PrivateRoute>
                <Accounts />
              </PrivateRoute>
            }
          />
          <Route path="/logout" element={<Logout />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
