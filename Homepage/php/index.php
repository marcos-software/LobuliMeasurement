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
	
	echo '<script>
	
	function rstudioweb() {
	  alert("\nLogin with\nusername: elvis\npassword: pass");
	  window.open("http://YOUR_R_STUDIO_WEB_URL_HERE.com:8787/");
	}
	
	</script>';
	
	require_once('db.php');
		
	$pdoLog                 = getPDO(1);		
	
	$statement              = $pdoLog->prepare("SELECT bAdmin, cMail FROM vUsers JOIN vSecurityTokens ON vUsers.cUserId = vSecurityTokens.cUserId WHERE vSecurityTokens.cIdentifier = :cIdentifier");
	$statement->execute(array('cIdentifier' => $_COOKIE['identifier']));
	$vUser	= $statement->fetch();
	
	$currentMail = $vUser['cMail'];
	
	echo '<body>';
	
	echo '<div class="container"><br/>';	
	
	echo '<body>';
		echo '<table>';
			echo '<tr>';
				echo '<td rowspan="2">';
					echo '<img src="../gfx/lobuliMeasurement.png" style="height: 100px;">&nbsp; &nbsp; ';
				echo '</td>';
				echo '<td>';
					echo '<h1>Lobules Measurement</h1>';
				echo '</td>';
			echo '</tr>';
			echo '<tr>';				
				echo '<td>';
					echo 'Signed in as: <i>'.$currentMail.'</i>';
				echo '</td>';
			echo '</tr>';	
		echo '</table>';
	echo '<br/><br/><center>';	
	
    echo '<a href="overview.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Show Measurements</button></a>';		
	echo '&nbsp; &nbsp; <a href="analytics.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Analyze Measurements</button></a>';	
    echo '&nbsp; &nbsp; <a href="#" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;" onclick="rstudioweb()">RStudio Web</button></a>';	
	echo '<br/><br/>';	
	echo '<a href="errors.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Show Errors</button></a>';	
	echo '&nbsp; &nbsp; <a href="exclusions.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Show Exclusions</button></a>';	
	echo '&nbsp; &nbsp; <a href="anomalies.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Show Statistical Anomalies</button></a>';
	echo '<br/><br/>';	
	echo '<a href="user.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Manage User</button></a>';	
	echo '&nbsp; &nbsp; <a href="logout.php" class="previous"><button type="button" class="btn btn-lg" style="width: 300px;">Sign out</button></a>';	
	echo '</center></div><br /><br />';
	readfile("../html/footer.html");
	echo '</body>';
	echo '</html>';		
	die();		
?>		
	</body>
</html>