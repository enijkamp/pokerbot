<?php
$action = $_GET['action'];
$username = $_GET['user'];
$ourFileName = $username."_sessions.txt";
$fh = fopen($ourFileName, 'a') or die("can't open file");
$remoteIP = $_SERVER['REMOTE_ADDR'];
$line = date(DATE_RFC822)."  ->   $remoteIP   ->    $action"."\r\n";
fwrite($fh, $line);
fclose($fh);
?>

