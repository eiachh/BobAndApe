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

	private void OnLoginSuccess(LoginAcceptCommand acceptBody)
	{
		var controller = GetNode<GameController>("/root/GameController");
		controller.OnLoginSuccess(Username, acceptBody.Body.UserID);
	}

	private async void _on_submit_button_pressed() 
	{
		ClientSocket.SendMessage(PackageFactory.CreateLoginPackage(Username)).Wait();
		object loginObj = await ClientSocket.ReceiveMessage();
		LoginAcceptCommand sdsdsd = loginObj as LoginAcceptCommand;
		OnLoginSuccess(sdsdsd);
	}
	
	private void _on_username_textbox_text_changed(string newText)
	{
		Username = newText;
		GD.Print($"{Username}");
	}
	
}
