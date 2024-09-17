import { useEffect } from "react";

const Logout = () => {
  useEffect(() => {
    // Clear any other session data if needed
    window.location.href = "/login"; // Redirect to login page
  }, []);

  return <div>Logging out...</div>;
};

export default Logout;
