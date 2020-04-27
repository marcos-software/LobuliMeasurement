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
	
    readfile("../html/head.html");	
		
	require_once('db.php');
		
	$pdoLog                 = getPDO(1);		
	
	$statement              = $pdoLog->prepare("SELECT bAdmin, cMail FROM vUsers JOIN vSecurityTokens ON vUsers.cUserId = vSecurityTokens.cUserId WHERE vSecurityTokens.cIdentifier = :cIdentifier");
	$statement->execute(array('cIdentifier' => $_COOKIE['identifier']));
	$vUser	= $statement->fetch();
	
	$currentMail = $vUser['cMail'];
	
	if($vUser['bAdmin'] != '1')
	{
		echo '<br/><br/><div class="alert alert-danger">
				<strong>You are not allowed to manage the user,<br />because your aren\'t an admin.</strong>
			</div><br/><br/>
			<center><a href="index.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show Menu</button></a></center>';
			
			readfile("../html/footer.html");
		
	} else {
	
		$statement              = $pdoLog->prepare("SELECT * FROM tUsers");
		$statement->execute();
		$vUsers                 = $statement->fetchAll();
			
		echo '<body>
	<div id="masthead">  
	  <div class="container">
	    <br>
		<a href="index.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show Menu</button></a><br /><br />
		<table class="table table-striped">
			<tbody>';
			
		foreach($vUsers as $key => $vUser) {
			
			$buttonText = "Deactivate";
			$buttonClass = "btn btn-danger";
			
			$link = 'activateUser.php?mail='.$vUser['cMail'];	
			
			
			if($vUser['bActive'] == '0'){
				$buttonText = "Activate";
				$buttonClass = "btn btn-success";
			}		

			if($currentMail == $vUser['cMail'])	{
					$buttonClass .= ' disabled';
					$link = '#';
			}	


			echo '<tr role="row">';			
				echo '<td>'.$vUser['cMail'].'</td>';
				echo '<td><a href="'.$link.'"><i class="fa fa-pencil"><button type="button" class="'.$buttonClass.'">'.$buttonText.'</button></i></a></td>';
			echo '</tr>';	
		}
			echo'
		  </tbody>
		</table></div></div>';
	}
	
	
	
?>		
	</body>
</html>