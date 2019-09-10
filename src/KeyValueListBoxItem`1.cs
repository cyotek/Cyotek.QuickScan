namespace Cyotek.QuickScan
{
  internal sealed class KeyValueListBoxItem<TValue>
  {
    #region Private Fields

    private string _name;

    private TValue _value;

    #endregion Private Fields

    #region Public Constructors

    public KeyValueListBoxItem(string name, TValue value)
    {
      _name = name;
      _value = value;
    }

    #endregion Public Constructors

    #region Public Properties

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public TValue Value
    {
      get { return _value; }
      set { _value = value; }
    }

    #endregion Public Properties

    #region Public Methods

    public override string ToString()
    {
      return _name;
    }

    #endregion Public Methods
  }
}