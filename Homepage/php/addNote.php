<?php
/**
 * #############################################################################
 * #                                                                           #
 * # copyright (c) 2019 marcos software, all rights reserved                   #
 * #                                                                           #
 * # this file may not be redistributed in whole or significant part           #
 * # and is subject to the marcos software license.                            #
 * #                                                                           #
 * # @author: marcos software, <info@marcos-software.de>                       #
 * # @copyright: 2019, marcos-software.de, http://www.marcos-software.de       #
 * #                                                                           #
 * #############################################################################
 */ 
    ini_set('session.cookie_lifetime', 60 * 60 * 24 * 7);  // 7 day cookie lifetime
	session_start();

	require_once('db.php');
	$pdo       = getPDO(1);

	if(!isset($_SESSION['userid']) && isset($_COOKIE['identifier']) && isset($_COOKIE['securitytoken'])) {
		
		$identifier        = $_COOKIE['identifier'];
		$securitytoken     = $_COOKIE['securitytoken'];

		$statement         = $pdo->prepare("SELECT * FROM vSecurityTokens WHERE cIdentifier = ?");
		$result            = $statement->execute(array($identifier));
		$securitytoken_row = $statement->fetch();

		if(sha1($securitytoken) !== $securitytoken_row['cSecurityToken']) {
			
			echo '<html lang="en">';
			readfile("../html/head.html");
			readfile("../html/invalidToken.html");
			
			die();
			
		} else { 

			require_once('utils.php');

			$neuer_securitytoken = random_string(); 

			$stmt                = $pdo->prepare("CALL spUpdateToken(?, ?)");

			$stmt->bindParam(3, sha1($neuer_securitytoken), PDO::PARAM_STR, 255);		
			$stmt->bindParam(2, $identifier, PDO::PARAM_STR, 255);

			$result              = $stmt->execute();

			setcookie("identifier", $identifier,time()+(3600*24*7), "", "", true, true);
			setcookie("securitytoken", $neuer_securitytoken,time()+(3600*24*7), "", "", true, true);

			$_SESSION['userid']  = $securitytoken_row['cUserId'];
		}
	}
 
	if(!isset($_SESSION['userid'])) {
		
		header("Location: https://YOUR_BASE_URL_HERE.com/php/login.php?".$_SERVER['QUERY_STRING']); 
		header("Connection: close"); 		
	}
?>
<?php

	$age           = $_POST['age'];
	$genotype      = $_POST['genotype'];
	$animal        = $_POST['animal'];
	$cutidentifier = $_POST['cutidentifier'];
	$cutId         = $_POST['cutid'];
	$note          = $_POST['note'];

	if
	(
		(is_null($age) == false && isset($age) && trim($age) !== '') &&
		(is_null($genotype) == false && isset($genotype) && trim($genotype) !== '') &&
		(is_null($animal) == false && isset($animal) && trim($animal) !== '') &&
		(is_null($cutidentifier) == false && isset($cutidentifier) && trim($cutidentifier) !== '') &&
		(is_null($cutId) == false && isset($cutId) && trim($cutId) !== '') &&
		(is_null($note) == false && isset($note) && trim($note) !== '')
	)	
	{
		require_once('db.php');
			
		$pdoLog                 = getPDO(1);	
		$mysqli                 = getPDO(1);	
		
		$statement              = $pdoLog->prepare("SELECT cMail FROM vUsers JOIN vSecurityTokens ON vUsers.cUserId = vSecurityTokens.cUserId WHERE vSecurityTokens.cIdentifier = :cIdentifier");
		$statement->execute(array('cIdentifier' => $_COOKIE['identifier']));
		$vUsers                  = $statement->fetch();

		$stmt = $mysqli->prepare("CALL spAddNote(?, ?, ?)");
		
		$stmt->bindParam(1, $cutId, PDO::PARAM_INT, 5);
		$stmt->bindParam(2, $note, PDO::PARAM_STR, 10000);
		$stmt->bindParam(3, $vUsers['cMail'], PDO::PARAM_STR, 50);
		
		$stmt->execute();		
	} 
	
	if (isset($_SERVER["HTTP_REFERER"])) 
	{			
		header("Location: " . $_SERVER["HTTP_REFERER"]);
	} 
	else 
	{
		echo '
		<html>
			<script>
				history.go(-1);
			</script>
		</html>';
	}	
?>		