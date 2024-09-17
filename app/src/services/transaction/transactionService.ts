import { Transaction } from "../../types/Transaction.ts";
import axios from "../axiosConfig.ts";

export const transferMoney = async (
  transaction: Transaction
): Promise<void> => {
  console.log("transacion is ", transaction);
  await axios.post("/transaction", transaction); // Relative path
};
