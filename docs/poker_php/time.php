<?php
$username = $_GET['user'];
$ourFileName = $username."_time.txt";
$totaltime = 0;
if(file_exists($ourFileName))
{
  $fh = fopen($ourFileName, "r") or die("can't open file");
  $totaltime = fread($fh, filesize ($ourFileName));
  fclose($fh);
}
$totaltime = $totaltime + 10;
$fh = fopen($ourFileName, "w");
fwrite($fh, $totaltime);
fclose($fh);
echo($totaltime);
?>

