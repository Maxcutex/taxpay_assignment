import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getAccounts } from "../services/account/accountService";
import { Account, AccountResponse } from "../types/Account";
import { transferMoney } from "../services/transaction/transactionService";
import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
} from "@mui/material";
import { Grid, Button, Form, Dropdown, Message, TextArea, Modal } from "semantic-ui-react";
import { Typography } from "antd";
import { Transaction } from "../types/Transaction";

const Accounts = () => {
  const [customerAccounts, setCustomerAccounts] = useState<Account[] | null>([]);
  const [taxAccounts, setTaxAccounts] = useState<Account[] | null>([]);
  const [openModal, setOpenModal] = useState(false);
  const [currentCustomerAccount, setCurrentCustomerAccount] = useState<Account | null>(null);
  const [destinationAccountId, setdestinationAcccountId] = useState("");
  const [amount, setAmount] = useState<number>(0);
  const [description, setDescription] = useState<string>("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const navigate = useNavigate();

  // Fetch accounts and tax accounts
  const fetchAccounts = async () => {
    try {
      const accounts: AccountResponse = await getAccounts(`?pageSize=10&pageNumber=1&type=0`);
      setCustomerAccounts(accounts.items || []);
    } catch (error) {
      console.error("Failed to fetch accounts", error);
    }
  };

  const fetchTaxAccounts = async () => {
    const accounts = await getAccounts("?pageSize=50&pageNumber=1&type=1");
    setTaxAccounts(accounts.items || null);
  };

  // Fetch the accounts on component mount
  useEffect(() => {
    fetchAccounts();
    fetchTaxAccounts();
  }, []);

  const taxAccountOptions = taxAccounts?.map((account) => ({
    key: account.id,
    text: `${account.title} - $${account.balance}`,
    value: account.id,
  }));

  const handleTransferClick = (account: Account) => {
    setOpenModal(true);
    setCurrentCustomerAccount(account);
  };

  const handleTransfer = async (e: React.FormEvent) => {
    e.preventDefault();
    if (currentCustomerAccount) {

      const transaction = {
        sourceAccountId: currentCustomerAccount?.id,
        destinationAccountId,
        amount,
        description,
      } as Transaction;

      try {
        await transferMoney(transaction);
        alert("Transfer successful");

        // Close the modal and reset fields
        setOpenModal(false);
        setAmount(0);
        setDescription("");
        setdestinationAcccountId("");

        // Refresh accounts
        await fetchAccounts();
        await fetchTaxAccounts();
        
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      } catch (error: any) {
        setErrorMessage(error.response.data.detail);
      }
    } 
  };

  // Handle logout function
  const handleLogout = () => {
    localStorage.removeItem("accessToken"); // Remove access token from localStorage
    navigate("/logout"); // Navigate to the logout route
  };

  return (
    <>
      {/* Logout button in the top right corner */}
      <div style={{ display: "flex", justifyContent: "flex-end", padding: "10px" }}>
        <Button primary onClick={handleLogout}>
          Logout
        </Button>
      </div>

      <Grid container spacing={4}>
        <Grid item sm={6}>
          <Typography style={{ fontWeight: 700, fontSize: 18 }}>Customer Accounts</Typography>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Title</TableCell>
                <TableCell>Balance</TableCell>
                <TableCell>Action</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {customerAccounts?.map((account) => (
                <TableRow key={account.id}>
                  <TableCell>{account.title}</TableCell>
                  <TableCell>${account.balance}</TableCell>
                  <TableCell>
                    <Button primary onClick={() => handleTransferClick(account)}>
                      Transfer Money
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Grid>

        <Grid item sm={6}>
          <Typography style={{ fontWeight: 700, fontSize: 18, paddingLeft: 8 }}>Tax Accounts</Typography>
          <Box sx={{ marginTop: 2 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Title</TableCell>
                  <TableCell>Balance</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {taxAccounts?.map((account) => (
                  <TableRow key={account.id}>
                    <TableCell>{account.title}</TableCell>
                    <TableCell>${account.balance}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </Box>
        </Grid>
      </Grid>

      <Modal
        open={openModal}
        closeIcon
        size="large"
        style={{ padding: 10 }}
        onClose={() => {
          setOpenModal(false);
          setAmount(0);
          setDescription("");
          setdestinationAcccountId("");
          setErrorMessage("");
        }}
      >
        <Box>
          <Form onSubmit={handleTransfer}>
            <Form.Field>
              <label>Amount</label>
              <input
                type="number"
                placeholder="Enter Amount"
                value={amount}
                onChange={(e) => setAmount(Number(e.target.value))}
                required
              />
            </Form.Field>

            {errorMessage && <Message negative>{errorMessage}</Message>}

            <Form.Field>
              <label>Select Tax Account</label>
              <Dropdown
                placeholder="Select Tax Account"
                fluid
                selection
                options={taxAccountOptions}
                value={destinationAccountId}
                onChange={(_e, { value }) => setdestinationAcccountId(value as string)}
                required
              />
            </Form.Field>

            <Form.Field>
              <label>Description</label>
              <TextArea
                placeholder="Add a description (optional)"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </Form.Field>

            <Button type="submit" primary>
              Transfer
            </Button>
          </Form>
        </Box>
      </Modal>
    </>
  );
};

export default Accounts;
