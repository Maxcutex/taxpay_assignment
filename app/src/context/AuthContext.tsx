/* eslint-disable @typescript-eslint/no-explicit-any */
// ** React Imports
import { createContext, ReactNode, useEffect, useState } from "react";

// ** Types
import { AuthValuesType, LoginParams, UserDataType } from "./types";
// import { axios } from "../util/http-client.ts";
import authConfig from "../configs/auth.ts";
import axios from "../services/axiosConfig.ts";
import { LoginResponse } from "../types/LoginResponse.ts";

// ** Defaults
const defaultProvider: AuthValuesType = {
  user: null,
  loading: true,
  setUser: () => null,
  setLoading: () => Boolean,
  login: async () => {
    return Promise.resolve(undefined);
  },
};

const AuthContext = createContext(defaultProvider);

type Props = {
  children: ReactNode;
};

const AuthProvider = ({ children }: Props) => {
  // ** States
  const [user, setUser] = useState<UserDataType | null>(defaultProvider.user);
  const [loading, setLoading] = useState<boolean>(defaultProvider.loading);

  // ** Hooks

  //const location = useLocation();

  useEffect(() => {
    const initAuth = async (): Promise<void> => {
      const storedToken = window.localStorage.getItem(
        authConfig.storageTokenKeyName
      )!;
      if (storedToken) {
        setLoading(true);
        await axios
          .get(authConfig.meEndpoint)
          .then(async (response) => {
            console.log("Response", response);
            // setUser({ ...response.data });
          })
          .catch(() => {
            // localStorage.removeItem(authConfig.storageUserDataKeyName);
            // localStorage.removeItem(authConfig.storageTokenKeyName);
            setUser(null);
            setLoading(false);
            // if (authConfig.onTokenExpiration === 'logout' && !location.pathname.includes('login')) {
            //   //history.push("/login");
            // }
          });
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const handleLogin = async (
    params: LoginParams
    // errorCallback?: ErrCallbackType
  ) => {
    setLoading(true);

    console.log("logingin in");
    try {
      const response = await axios.post<LoginResponse>(
        authConfig.loginEndpoint,
        {
          username: params.email,
          password: params.password,
          grant_type: "password",
          scope: process.env.VITE_PUBLIC_CLIENT_SCOPES,
          client_id: process.env.VITE_PUBLIC_CLIENT,
          client_secret: process.env.VITE_PUBLIC_JWT_SECRET,
        },
        {
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
          },
        }
      );

      console.log("Response", response);
      return response;
    } catch (err: any) {
      console.error("Failed", err);
      return err?.response || "Unable to connect";
    }

  };

  const values = {
    user,
    loading,
    setUser,
    setLoading,
    login: handleLogin,
  };

  return <AuthContext.Provider value={values}>{children}</AuthContext.Provider>;
};

export { AuthContext, AuthProvider };
