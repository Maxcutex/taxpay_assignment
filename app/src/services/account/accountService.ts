import axios from "../axiosConfig";
import { AccountResponse } from "../../types/Account";
import { GET_ACCOUNTS } from "../servicesUrl";

export const getAccounts = async (params: string): Promise<AccountResponse> => {
  const response = await axios.get<AccountResponse>(`${GET_ACCOUNTS}${params}`);
  return response.data;
};
