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

<html lang="en">

<?php

	$age           = $_GET['age'];
	$genotype      = $_GET['genotype'];
	$animal        = $_GET['animal'];
	$cutidentifier = $_GET['cutidentifier'];
	
    readfile("../html/head.html");	
		
		require_once('db.php');
			
		$pdoLog    = getPDO(1);
		
		$statement = $pdoLog->prepare("select * from vCut where bExcludeFromAnalysis = 1");
		$statement->execute();
		$vCut      = $statement->fetchAll();
		
		require_once('customers.php');	
		
		generatCustomerGrid($vCut);

		echo '<br /><br />';
		readfile("../html/footer.html");
		echo '</body>';
		echo '</html>';		
		die();		
		

?>		
	</body>
</html>