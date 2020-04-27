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
		
	$pdoLog     = getPDO(1);
	
	$statement  = $pdoLog->prepare("SELECT DISTINCT kAge FROM vCut WHERE bExcludeFromAnalysis = 0 ORDER BY kAge");
	$statement->execute();
	$vCutAge    = $statement->fetchAll();
	
	$statement  = $pdoLog->prepare("SELECT DISTINCT cAnimal FROM vCut WHERE bExcludeFromAnalysis = 0 ORDER BY cAnimal");
	$statement->execute();
	$vCutAnimal = $statement->fetchAll();
	
	$statement  = $pdoLog->prepare("SELECT DISTINCT cLayer FROM vCut WHERE bExcludeFromAnalysis = 0 ORDER BY cLayer");
	$statement->execute();
	$vCutLayer = $statement->fetchAll();
	
	$analyzeMethods = array();
	array_push($analyzeMethods, 'Mittelwert');
	array_push($analyzeMethods, 'Varianz');
	array_push($analyzeMethods, 'Standard Abweichung');
	array_push($analyzeMethods, 'Standard Fehler Mittelwert');
	array_push($analyzeMethods, 'Standard Fehler Median');
	array_push($analyzeMethods, 'Varianz Koeffizient');
	array_push($analyzeMethods, 'Quartil Dispersionskoeffizient');
	array_push($analyzeMethods, 'Minimum');
	array_push($analyzeMethods, 'Maximum');
	array_push($analyzeMethods, 'Median');
	array_push($analyzeMethods, 'Spannweite');
	array_push($analyzeMethods, 'Mittlere Abweichung');
	array_push($analyzeMethods, 'Median Abweichung');
		
	echo '<div class="container">';	
	
		echo '<br/><a href="index.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show Menu</button></a>';		
	
		echo '<h1>Filters:</h1><br/><br/>';	
		
		echo '<form class="form-inline" action="analyticsResult.php" method="post">';
		
			echo '<div class="form-group">';			
				echo '<label for="ageFrom">Age from &nbsp;</label>';
				echo '<select id="ageFrom" name="ageFrom" class="form-control">';  
						
				foreach($vCutAge as $key => $vCut) {
					
					if($vCut['kAge'] == ($vCutAge[0])['kAge'])
					{
						echo '<option selected value="'.$vCut['kAge'].'">'.$vCut['kAge'].'</option>';
					}
					else
					{
						echo '<option value="'.$vCut['kAge'].'">'.$vCut['kAge'].'</option>';
					}
				}					
				echo'</select>';				
			echo '</div>';
			
			echo '<div class="form-group">';
				echo '<label for="ageTo">&nbsp; to &nbsp;</label>';
				echo '<select id="ageTo" name="ageTo" class="form-control">';

				$lastSelected = false;	        		
				foreach($vCutAge as $key => $vCut) {
					
					if($vCut['kAge'] == (end($vCutAge))['kAge'])
					{
						echo '<option selected value="'.$vCut['kAge'].'">'.$vCut['kAge'].'</option>';
					}
					else
					{
						echo '<option value="'.$vCut['kAge'].'">'.$vCut['kAge'].'</option>';
					}
				}					
				echo'</select>';
			echo '</div>';
			
			echo '<br/><br/>';	
			
			echo '<b>Animals:</b> &nbsp;';
			
			foreach($vCutAnimal as $key => $vCut) {
				
				echo '<div class="form-group form-check form-check-inline">';	
				echo '<input class="form-check-input" type="checkbox" id="checkboxAnimal_'.$vCut['cAnimal'].'" name="animals[]" value="\''.$vCut['cAnimal'].'\'" checked>';
				echo '<label class="form-check-label" for="checkboxAnimal_'.$vCut['cAnimal'].'">&nbsp;'.$vCut['cAnimal'].' &nbsp; &nbsp; </label>';
				echo '</div>';
			}
			
			echo '<br/><br/>';	
			
			echo '<b>Layer:</b> &nbsp;';
			
			foreach($vCutLayer as $key => $vCut) {
				
				echo '<div class="form-group form-check form-check-inline">';	
				echo '<input class="form-check-input" type="checkbox" id="checkboxLayer_'.$vCut['cLayer'].'" name="layer[]" value="\''.$vCut['cLayer'].'\'" checked>';
				echo '<label class="form-check-label" for="checkboxLayer_'.$vCut['cLayer'].'">&nbsp;'.$vCut['cLayer'].' &nbsp; &nbsp; </label>';
				echo '</div>';
			}
			
			echo '<br/><br/>';	
			
			echo '<h1>Statistical Methods:</h1> &nbsp;';
			
			foreach($analyzeMethods as $method) {
				
				$imageName = str_replace(' ', '_', strtolower($method)).'.jpg';
				
				echo '<div class="form-group form-check form-check-inline">';	
				echo '<input class="form-check-input" type="checkbox" id="checkboxStatisticalMethod_'.$method.'" name="statisticalMethod[]" value="\''.$method.'\'" checked>';
				echo '<label class="form-check-label" for="checkboxStatisticalMethod_'.$method.'"> &nbsp;'.$method.' <a data-toggle="tooltip" title="<img src=\'../gfx/'.$imageName.'\' />"><img src="../gfx/info.png" style="width:20px;height:20px;"></a></label>';
				echo '</div> &nbsp; &nbsp;  &nbsp; ';
			}
			
			echo '<br/><br/><input type="submit" class="btn btn-lg" value="analyse">';
			
		echo '</form>';		
		
	echo '</div><br /><br />';
	readfile("../html/footer.html");
	echo '</body>';
	echo '</html>';		
	die();		

?>		
	</body>
</html>