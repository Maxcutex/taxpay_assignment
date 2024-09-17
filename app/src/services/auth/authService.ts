// services/auth/authService.ts

import axios from "../axiosConfig";
import { AuthResponse, LoginCredentials } from "../../types/Auth";

export const login = async (
  credentials: LoginCredentials
): Promise<AuthResponse> => {
  // Convert the credentials object to x-www-form-urlencoded format
  const params = new URLSearchParams();
  params.append("username", credentials.username);
  params.append("password", credentials.password);
  params.append("grant_type", credentials.grant_type || "password");
  params.append("scope", credentials.scope || "");
  params.append("client_id", credentials.client_id);
  params.append("client_secret", credentials.client_secret);

  // Send POST request with x-www-form-urlencoded header
  const response = await axios.post<AuthResponse>("/connect/token", params, {
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
  });
  console.log("Response: ", response);
  return response.data;
};
