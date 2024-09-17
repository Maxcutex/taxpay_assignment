import { LockOutlined } from "@mui/icons-material";
import {
  Container,
  CssBaseline,
  Box,
  Avatar,
  Typography,
  TextField,
  Button,
} from "@mui/material";
import { FormEvent, useState } from "react";
import { useAuth } from "../hooks/useAuth.ts";
import { useNavigate } from "react-router-dom";
import { Alert } from "antd";
import BeatLoader from "react-spinners/ClipLoader";
import { LoginResponse } from "../types/LoginResponse.ts";

const Login = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [hasError, setHasError] = useState(false);
  const auth = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: FormEvent<HTMLFormElement>) => {
    setSubmitting(true);
    setHasError(false);
    e.preventDefault();
    console.log(
      "handling login",
      email,
      password,
      process.env.VITE_PUBLIC_CLIENT_SCOPES
    );

    const loginService: LoginResponse | undefined = await auth.login({
      email,
      password,
    });
    switch (loginService?.status) {
      case 200:
        setSubmitting(false);
        setHasError(false);
        console.log("loginService", loginService);
        localStorage.setItem("accessToken", loginService?.data?.access_token);
        navigate("/");
        break;
      case 400:
        setSubmitting(false);
        setHasError(true);
        setErrorMessage("Invalid Email or Password");
        break;
      default:
        setSubmitting(false);
        setHasError(true);
        setErrorMessage("Unable to process request");
        break;
    }
  };

  return (
    <>
      <Container maxWidth="xs">
        <CssBaseline />
        <Box
          sx={{
            mt: 20,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          <Avatar sx={{ m: 1, bgcolor: "primary.light" }}>
            <LockOutlined />
          </Avatar>
          <Typography variant="h5">Login</Typography>
          <Box sx={{ mt: 1 }}>
            {hasError && (
              <Alert message={errorMessage} showIcon={true} type="error" />
            )}
            <form onSubmit={handleLogin}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoFocus
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />

              <TextField
                margin="normal"
                required
                fullWidth
                id="password"
                name="password"
                label="Password"
                type="password"
                value={password}
                onChange={(e) => {
                  setPassword(e.target.value);
                }}
              />

              <Button
                fullWidth
                type="submit"
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={submitting}
              >
                {submitting ? <BeatLoader /> : "Login"}
              </Button>
            </form>
          </Box>
        </Box>
      </Container>
    </>
  );
};

export default Login;
