namespace UPDB.CoreHelper.UsableMethods.Structures
{
	/// <summary>
	/// act like a boolean when set, but automatically reseted when getted
	/// </summary>
	[System.Serializable]
	public struct Trigger
	{
		[UnityEngine.SerializeField]
		private bool _value;

		public bool Read
		{
			get
			{
				bool value = _value;
				_value = false;
				return value; 
			}
		}

		public bool TAMERE
		{
			get { return _value; }
			set { _value = value; }
		}

        public Trigger(bool value)
		{
			_value = value;
		}

		public void Enable()
		{
			_value = true;
		}

		public void Reinitialize()
		{ 
			_value = false; 
		}

	} 
}
