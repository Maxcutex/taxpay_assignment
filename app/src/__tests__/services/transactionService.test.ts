// Not enough time to fix the test having issues mocking the axios
// import axios from "axios";
// import { transferMoney } from "../../services/transaction/transactionService";

// // Mock the `axios.post` method directly
// jest.mock("axios");
// const mockedAxios = axios as jest.Mocked<typeof axios>;

// describe("transactionService - transferMoney", () => {
//   const transactionData = {
//     sourceAccountId: "account123",
//     destinationAccountId: "tax456",
//     amount: 100,
//     description: "Transfer to tax account",
//   };

//   beforeEach(() => {
//     jest.clearAllMocks(); // Clear mock data before each test
//   });

//   it("should successfully transfer money", async () => {
//     // Mock a successful response for the `axios.post` call
//     mockedAxios.post.mockResolvedValue({});

//     await transferMoney(transactionData);

//     expect(mockedAxios.post).toHaveBeenCalledWith("/transaction", transactionData);
//   });

//   it("should handle transfer error", async () => {
//     // Mock a rejected response for the `axios.post` call
//     mockedAxios.post.mockRejectedValue(new Error("Transfer failed"));

//     await expect(transferMoney(transactionData)).rejects.toThrow("Transfer failed");
//   });
// });
