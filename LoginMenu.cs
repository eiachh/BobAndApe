using Godot;
using System;
using System.ComponentModel;
using Package;
using System.Threading.Tasks;

public partial class LoginMenu : Control
{
	public string Username { get; set; }
	private LineEdit _usernameTextbox;
	
	public override void _Ready() 
	{
		_usernameTextbox = GetNode<LineEdit>("UsernameTextbox");
	}

  

	private async void _on_submit_button_pressed() 
	{
		ClientSocket.SendMessage(PackageFactory.CreateLoginPackage(Username)).Wait();
		ClientSocket.ReceiveMessage();
	}
	
	private void _on_username_textbox_text_changed(string newText)
	{
		Username = newText;
		GD.Print($"{Username}");
	}
}
