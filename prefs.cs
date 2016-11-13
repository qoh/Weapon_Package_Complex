if(isFile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	if(!$RTB::RTBR_ServerControl_Hook) exec("Add-Ons/System_ReturnToBlockland/hooks/ServerControl.cs");
	RTB_registerPref("Max Spent Shells", "Complex Weapons | Optimization", "$Pref::Server::ComplexWeapons::Optimization::ShellLimit", "int 0 64", "Weapon_Package_Complex", 30, 0, 0);

	RTB_registerPref("Starter Ammo (357)", "Complex Weapons | Ammo", "$Pref::Server::ComplexWeapons::Ammo::357", "int 0 36", "Weapon_Package_Complex", 0, 0, 0);
	RTB_registerPref("Starter Ammo (Buckshot)", "Complex Weapons | Ammo", "$Pref::Server::ComplexWeapons::Ammo::Buckshot", "int 0 36", "Weapon_Package_Complex", 0, 0, 0);
	RTB_registerPref("Weapons Spawn w/ Mags", "Complex Weapons | Ammo", "$Pref::Server::ComplexWeapons::Ammo::GunMags", "bool", "Weapon_Package_Complex", false, 0, 0);

	RTB_registerPref("Show Gun Help", "Complex Weapons | Display", "$Pref::Server::ComplexWeapons::Display::ShowGunHelp", "bool", "Weapon_Package_Complex", true, 0, 0);
}
else
{
	if($Pref::Server::ComplexWeapons::Optimization::ShellLimit $= "") $Pref::Server::ComplexWeapons::Optimization::ShellLimit = 30;
	if($Pref::Server::ComplexWeapons::Ammo::357 $= "") $Pref::Server::ComplexWeapons::Ammo::357 = 0;
	if($Pref::Server::ComplexWeapons::Ammo::Buckshot $= "") $Pref::Server::ComplexWeapons::Ammo::Buckshot = 0;
	if($Pref::Server::ComplexWeapons::Ammo::GunMags $= "") $Pref::Server::ComplexWeapons::Ammo::GunMags = 0;
	if($Pref::Server::ComplexWeapons::Display::ShowGunHelp $= "") $Pref::Server::ComplexWeapons::Display::ShowGunHelp = true;
}