export type LoginResponse = {
  status: number;
  data: {
    access_token: string;
    token_type?: string;
    expires_in?: number;
  };
};
