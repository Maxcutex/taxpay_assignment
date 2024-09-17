
 
import { render, screen, waitFor } from '@testing-library/react';
import { getAccounts } from '../../services/account/accountService';
import { transferMoney } from '../../services/transaction/transactionService';
import { MockIndex } from '../__mocks__/MockIndex';
import userEvent from '@testing-library/user-event';
import Accounts from '../../components/Accounts'; 

 
jest.mock('../../services/account/accountService');
jest.mock('../../services/transaction/transactionService');

const renderComponent = () => {
  return render(
    <MockIndex>
      <Accounts />
    </MockIndex>
  );
};

describe('Accounts Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('renders customer and tax accounts', async () => {

    (getAccounts as jest.Mock).mockResolvedValueOnce({
      items: [
        { id: '1', title: 'Customer Account 1', balance: 1000, type: 'Customer' },
        { id: '2', title: 'Customer Account 2', balance: 2000, type: 'Customer' },
      ],
    });

    (getAccounts as jest.Mock).mockResolvedValueOnce({
      items: [
        { id: '3', title: 'Tax Account 1', balance: 500, type: 'Tax' },
        { id: '4', title: 'Tax Account 2', balance: 1000, type: 'Tax' },
      ],
    });

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('Customer Accounts')).toBeInTheDocument();
      expect(screen.getByText('Tax Accounts')).toBeInTheDocument();
    });

    expect(screen.getByText('Customer Account 1')).toBeInTheDocument();
    expect(screen.getByText('Customer Account 2')).toBeInTheDocument();
    expect(screen.getByText('Tax Account 1')).toBeInTheDocument();
    expect(screen.getByText('Tax Account 2')).toBeInTheDocument();
  });

  test('opens the transfer modal when "Transfer Money" is clicked', async () => {
    (getAccounts as jest.Mock).mockResolvedValueOnce({
      items: [{ id: '1', title: 'Customer Account 1', balance: 1000, type: 'Customer' }],
    });

    (getAccounts as jest.Mock).mockResolvedValueOnce({
      items: [{ id: '3', title: 'Tax Account 1', balance: 500, type: 'Tax' }],
    });

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('Customer Account 1')).toBeInTheDocument();
    });

    const transferButton = screen.getByText('Transfer Money');
    userEvent.click(transferButton);

    expect(screen.getByText('Amount')).toBeInTheDocument();
    expect(screen.getByText('Select Tax Account')).toBeInTheDocument();
  });

  test('submits the transfer form and closes the modal', async () => {
    (getAccounts as jest.Mock).mockResolvedValueOnce({
      items: [{ id: '1', title: 'Customer Account 1', balance: 1000, type: 'Customer' }],
    });

    (getAccounts as jest.Mock).mockResolvedValueOnce({
      items: [{ id: '3', title: 'Tax Account 1', balance: 500, type: 'Tax' }],
    });

    (transferMoney as jest.Mock).mockResolvedValueOnce({});

    renderComponent();

    await waitFor(() => {
      expect(screen.getByText('Customer Account 1')).toBeInTheDocument();
    });

    const transferButton = screen.getByText('Transfer Money');
    userEvent.click(transferButton);

    const amountInput = screen.getByPlaceholderText('Enter Amount');
    const descriptionInput = screen.getByPlaceholderText('Add a description (optional)');

    userEvent.type(amountInput, '200');
    userEvent.type(descriptionInput, 'Test transfer');

    const taxAccountDropdown = screen.getByText('Select Tax Account');
    userEvent.click(taxAccountDropdown);
    userEvent.click(screen.getByText('Tax Account 1 - $500')); // Select option

    const submitButton = screen.getByText('Transfer');
    userEvent.click(submitButton);

    await waitFor(() => {
      expect(transferMoney).toHaveBeenCalledWith({
        sourceAccountId: '1',
        destinationAccountId: '3',
        amount: 200,
        description: 'Test transfer',
      });
    });

    expect(screen.queryByText('Amount')).not.toBeInTheDocument();
  });
});
