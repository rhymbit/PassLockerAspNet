namespace PassLocker.Services.Token
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a json web token.
        /// </summary>
        /// <param name="username">Current user's username. Fetch if from database and pass it to this method.</param>
        /// <param name="sec">Fetch user's password's hash value and pass it as this argument.</param>
        /// <returns>String value of the generated jwt.</returns>
        string CreateToken(string username, string sec);

        bool ValidateToken(string token, string sec);
    }
}