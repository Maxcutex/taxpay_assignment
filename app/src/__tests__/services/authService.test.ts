import axios from "axios";
import { login } from "../../services/auth/authService";
import { LoginCredentials, AuthResponse } from "../../types/Auth";

jest.mock("axios"); 
const mockedAxios = axios as jest.Mocked<typeof axios>;

describe("authService - login", () => {
  const credentials: LoginCredentials = {
    username: "testuser",
    password: "password123",
    grant_type: "password",
    scope: "test_scope",
    client_id: "test_client_id",
    client_secret: "test_client_secret",
  };

  const mockResponse: AuthResponse = {
    email: "testuser@example.com",
    firstName: "Test",
    id: "12345",
    lastName: "User",
    username: "testuser",
  };

  it("should successfully login and return user data", async () => {
    mockedAxios.post.mockResolvedValue({ data: mockResponse });

    const result = await login(credentials);

    expect(mockedAxios.post).toHaveBeenCalledWith(
      "/login",
      expect.any(URLSearchParams),
      {
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
      }
    );
    expect(result).toEqual(mockResponse);
  });

  it("should handle login error", async () => {
    mockedAxios.post.mockRejectedValue(new Error("Login failed"));

    await expect(login(credentials)).rejects.toThrow("Login failed");
  });
});
