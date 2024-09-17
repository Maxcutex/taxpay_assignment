export type Account = {
  id: string;
  title: string;
  balance: number;
  type: "Customer" | "Tax";
};

export type AccountResponse = {
  items?: Account[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
};
