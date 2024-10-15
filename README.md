# Carsties

This project is the demo for an auction wep app. This application is built based on microservice architecture. The backend system was completed using C# .NET and the frontend was developed by Next.js.

## Table of Contents

- [Installation](#installation)
- [Running the App Locally](#running-the-development-server)
- [Running Tests](#running-tests)

## Installation

To set up the project, follow these steps:

**1. Clone the repository**
```bash
git clone https://github.com/AndyTranPro/Carsties.git
cd ./frontend/web-app
```

**2. Install all the necessary packages in the root directory**
```bash
npm install
```

## Running the App Locally (Docker)

To build the docker containers for the application, you need to be at the root directory of this project and run the following commands:

```bash
docker compose build
docker compose up -d
```
The application will be available at https://app.carsties.local/

### Running Tests
TODO

## License
This project is licensed under the MIT License. See the [LICENSE](https://opensource.org/license/mit) for more details.
