public interface IGroupable {
	public bool Active { get; set; }
	public bool Blocked { get; set; }
	public void Reset ();
	public void OnCompleted (double duration);

}

