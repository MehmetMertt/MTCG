# Monster Trading Card Game (MTCG)

Monster Trading Card Game (MTCG) is a REST-based server application for a card game where users can register, manage cards, trade, and battle with other players. The game features a wide variety of monster and spell cards, each with different attributes and effects, allowing for strategic gameplay.

It was a mandatory task not to use any REST Framework like ASP.net

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Installation](#installation)
- [API Endpoints](#api-endpoints)

## Features

- User registration and authentication
- Card management (view, trade, and acquire cards)
- Deck creation and battle system
- Scoreboard with ELO-based ranking system
- Trade system with customizable offers and requests
- User profile with editable information
- RESTful API for interacting with the server

## Technologies

- **C#**: Core language for the server implementation
- **PostgreSQL**: Database to store users, cards, and battle data
- **CURL**: Integration testing using provided scripts
- **REST**: HTTP-based API design
- **Unit Testing**: 20+ unit tests to ensure application reliability

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/MehmetMertt/MTCG.git
   ```
2. Navigate to the project directory:

   ```bash
   cd MTCG
   ```

3. Set up the PostgreSQL database:

   - Configure the database connection in the project.

4. Build and run the project:

   ```bash
   dotnet build
   dotnet run
   ```

5. Test the application using the provided CURL script for integration tests.

## API Endpoints

The MTCG application exposes several API endpoints to manage users, cards, and battles. Below are some key examples:

- **User Registration**:  
  `POST /users`  
  Registers a new user with a unique username and password.
- **Login**:  
  `POST /sessions`  
  Logs in the user and returns a token for authenticated actions.
