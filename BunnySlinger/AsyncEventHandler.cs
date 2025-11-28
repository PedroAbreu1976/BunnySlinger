namespace BunnySlinger;

public delegate Task AsyncEventHandler(object sender, EventArgs args);
public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs args) where TEventArgs : EventArgs;


