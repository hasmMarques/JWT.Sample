namespace JWT.Interfaces
{
	public interface IJWTService
	{
		#region Public Methods

		string GetToken();
        void WriteLineToken();

        #endregion
    }
}
