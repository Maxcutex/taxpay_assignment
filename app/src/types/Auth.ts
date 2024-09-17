// models/Auth.ts
export type LoginCredentials = {
    username: string;
    password: string;
    grant_type: string;
    scope: string;
    client_id: string;
    client_secret: string;
  }
  
  export type AuthResponse = {
    email: string;
    firstName: string;
    id: string;
    lastName: string;
    username: string;
  }
  