import { LoginResponse } from "../types/LoginResponse";

export type ErrCallbackType = (err: { [key: string]: string }) => void;

export type LoginParams = {
  email: string;
  password: string;
};

export type AuthValuesType = {
  loading: boolean;
  user: UserDataType | null;
  setLoading: (value: boolean) => void;
  setUser: (value: UserDataType | null) => void;
  login: (
    params: LoginParams,
    errorCallback?: ErrCallbackType
  ) => Promise<LoginResponse | undefined>;
};

export type UserDataType = {
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  id: string;
};
