# PassLocker <img src="https://github.com/prateek332/PassLocker/blob/main/Icons/brand-icon.ico" width="65" align="center" />

- ### An API project for securly storing it's users secret information like passwords.
- ### This project uses industry/federal certified encryption algorithms to store information.
- #### This project was created to be used with PassLocker frontend app, but it can be used with any applicaton or as a standalone api. PassLocker frontend app can be found <a href="https://github.com/prateek332/PassLockerFE" target="_blank" rel="noopener noreferrer">here</a>.
- ##### ⚠️ **WARNING: -** You're free to use this project as you want but use it at your own risk in a production environment. I haven't certified this project for it's encryption functionalities through any federal institution and would not take responsiblity if any data gets leaked or hacked by someone.

<hr/>

##### Update your own configurations in <a href="https://github.com/prateek332/PassLocker/blob/main/PassLocker/MyConfig.json">MyConfig.json</a> file before running the project. Also update information in <a href="https://github.com/prateek332/PassLocker/blob/main/PassLockerDatabase/PassLockerDbContext.cs">DbContext</a> class of the `PassLocker` database folder.

## API Endpoints

### 1. Login endpoints

#### `api/login/google-login`
- `POST` method. This method can be used to login it' users using current Google's O auth functinalities.
- This method requires a GoogleOAuth token to be sent in the request body, which would be validated.
- Returns basic details of a user on successful authentication.
- requset body
  - ```
    {
      "myGoogleToken"
    }
    ```
    
### 2. User endpoints

#### `api/user/all-users`
- `GET` method
- Returns all the in the database

#### `api/user/{id}`
- `GET` method
- Returns user from the database with the provided id

#### `api/user/create-user`
- `POST` method .Creates a new user in the database.
- This method requires user's basic information to be sent in the request body.
- request body
  - ```
      {
        "username": "string",
        "password": "string",
        "email": "string",
        "secret": "string",
        "name": "string",
        "gender": "string",
        "location": "string"
      }
    ```

#### `api/user/update-user`
- `POST` method .Updates an existing user in the database.
- This method requires user's basic information to be sent in the request body.
- request body
  - ```
      {
        "username": "string",
        "password": "string",
        "email": "string",
        "secret": "string",
        "name": "string",
        "gender": "string",
        "location": "string"
      }
    ```

#### `api/user/{id}/delete-user`
- `DELETE` method. Deletes the user with the provided id from the database

### 3. Password endpoints

#### `api/password/{id}/verify-user
- `POST` method. Verifies user with the provided id in the database.
- This method requires user's password and secret to be sent in the request body.
- Returns a `password-token` in response which can be used to verify a user instead of his/her `password` & `secret`.
- request body
  - ```
      {
        "password": "string",
        "secret": "string"
      }
    ```
#### `api/password/{id}/verify-token`
- `POST` method. Verifies the password-token that's created and sent when `api/password/{id}/verify-user` endpoint is used.
- This method requires a `password-token` to be sent in request body.
- request body
    - ```
        {
          "passwordToken": "string"
        }
      ```
 
#### `api/password/{id}/get-passwords
- `GET` method. Get's all the stored password for a user with the provided `id`.

#### `api/password/{id}/create-passwords
- `POST` method. This method can be used to: -
  - `Create` passwords
  - `Update` passwords
  - `Delete` passwords
- Returns all the stored passwords for the user in the response body (just like `get-passwords` endpoint)
- This method is designed to be used with PassLocker frontend application called *PassLockerFE* that can be found 
  <a href="https://github.com/prateek332/PassLockerFE" target="_blank" rel="noopener noreferrer">here</a>.
- Client would send the password to create, update or delete all in the same request body.
- request body
  - ```
    {
      "domainName1": "password1"
      "domainName2": "password2"
      ....
    }
    ```
- Database would save just one **password** for one **domain**, i.e. unique key-value pairs.
- ALl the passwords are encrypted before they are saved to database and decrypted when returned in the response body.
