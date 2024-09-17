# Use a Node.js base image
FROM node:18 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy only package.json and yarn.lock (or package-lock.json if you use npm)
COPY package.json yarn.lock ./

# Install dependencies inside the container
RUN yarn install --frozen-lockfile # Or "npm install" if you're using npm

# Copy the rest of the application code
COPY . .

# Build the application
RUN yarn build # Or "npm run build" for npm

# Use Nginx to serve the built static files
FROM nginx:alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
