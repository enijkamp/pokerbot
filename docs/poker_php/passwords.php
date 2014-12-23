<?php
function encrypt($string) 
{
  for ($i=0;$i<strlen($string);$i++) 
  {
      $chr = $string{$i};
      $ord = ord($chr);
      $string{$i} = chr($ord-1);
  }
  return $string;
}

$username = $_GET["user"];
$version = $_GET["version"];
if($version == "2")
{
  if($username == "erik") echo encrypt("nijkamp123");
  if($username == "erik2") echo encrypt("nijkamp123");
  if($username == "tim") echo encrypt("12poker56");
}
?>
