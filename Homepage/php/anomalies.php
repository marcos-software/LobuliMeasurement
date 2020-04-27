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
	array_push($analyzeMethods, 'Three Sigma Rule');
	array_push($analyzeMethods, 'IQR Outlier Test');
	array_push($analyzeMethods, 'IQR Outlier Test 1.5');
	
	$propertiesGlobal = array();
	$propertiesFissure = array();
	$propertiesLobule = array();

	#region global					
	$statement      = $pdoLog->prepare('SELECT DISTINCT cResultType, cIdentifier, cKey FROM vResult WHERE cIdentifier IS NULL ORDER BY cKey');
	$statement->execute();
	$vCutKeysGlobal = $statement->fetchAll();

	foreach($vCutKeysGlobal as $key => $vCut) {
		
		$property = new stdClass();
		$property->cResultType = $vCut['cResultType'];
		$property->cIdentifier = $vCut['cIdentifier'];
		$property->cKey = $vCut['cKey'];
		
		if (strpos($property->cKey, 'number of') !== false) {
			continue;
		}
							
		array_push($propertiesGlobal, json_encode($property));			
	}
	#endregion global
	
	#region fissure	& lobule
	$statement      = $pdoLog->prepare('SELECT DISTINCT cResultType, cIdentifier, cKey FROM vResult WHERE cResultType = \'fissure\' AND cIdentifier IS NOT NULL AND CONCAT(\'\',cIdentifier * 1) != cIdentifier ORDER BY cResultType, cIdentifier, cKey');
	$statement->execute();
	$vCutKeysGlobal = $statement->fetchAll();

	foreach($vCutKeysGlobal as $key => $vCut) {
		
		$property = new stdClass();
		$property->cResultType = $vCut['cResultType'];
		$property->cIdentifier = $vCut['cIdentifier'];
		$property->cKey = $vCut['cKey'];
		
		if (strpos($property->cKey, 'number of') !== false) {
			continue;
		}
							
		array_push($propertiesFissure, json_encode($property));			
	}
	
	$statement      = $pdoLog->prepare('SELECT DISTINCT cResultType, cIdentifier, cKey FROM vResult WHERE cResultType = \'fissure\' AND cIdentifier IS NOT NULL AND CONCAT(\'\',cIdentifier * 1) = cIdentifier ORDER BY cResultType, cIdentifier, cKey');
	$statement->execute();
	$vCutKeysGlobal = $statement->fetchAll();

	foreach($vCutKeysGlobal as $key => $vCut) {
		
		$property = new stdClass();
		$property->cResultType = $vCut['cResultType'];
		$property->cIdentifier = $vCut['cIdentifier'];
		$property->cKey = $vCut['cKey'];
		
		if (strpos($property->cKey, 'number of') !== false) {
			continue;
		}
							
		array_push($propertiesFissure, json_encode($property));			
	}
	
	$statement      = $pdoLog->prepare('SELECT DISTINCT cResultType, cIdentifier, cKey FROM vResult WHERE cResultType = \'lobule\' AND cIdentifier IS NOT NULL AND CONCAT(\'\',cIdentifier * 1) != cIdentifier ORDER BY cResultType, cIdentifier, cKey');
	$statement->execute();
	$vCutKeysGlobal = $statement->fetchAll();

	foreach($vCutKeysGlobal as $key => $vCut) {
		
		$property = new stdClass();
		$property->cResultType = $vCut['cResultType'];
		$property->cIdentifier = $vCut['cIdentifier'];
		$property->cKey = $vCut['cKey'];
		
		if (strpos($property->cKey, 'number of') !== false) {
			continue;
		}
							
		array_push($propertiesLobule, json_encode($property));			
	}
	
	$statement      = $pdoLog->prepare('SELECT DISTINCT cResultType, cIdentifier, cKey FROM vResult WHERE cResultType = \'lobule\' AND cIdentifier IS NOT NULL AND CONCAT(\'\',cIdentifier * 1) = cIdentifier ORDER BY cResultType, cIdentifier, cKey');
	$statement->execute();
	$vCutKeysGlobal = $statement->fetchAll();

	foreach($vCutKeysGlobal as $key => $vCut) {
		
		$property = new stdClass();
		$property->cResultType = $vCut['cResultType'];
		$property->cIdentifier = $vCut['cIdentifier'];
		$property->cKey = $vCut['cKey'];
		
		if (strpos($property->cKey, 'number of') !== false) {
			continue;
		}
							
		array_push($propertiesLobule, json_encode($property));			
	}
	#endregion fissure			

		
	echo '<div class="container">';	
	
		echo '<br/><a href="index.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show Menu</button></a>';		
	
		echo '<h1>Filters:</h1><br/><br/>';	
		
		echo '<form class="form-inline" action="anomaliesResult.php" method="post">';
		
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
				
				$link = '';
				$name = '';
				$checked = 'checked';
				if($method == 'Three Sigma Rule') {
					
					$checked = '';
					$name = $method;
					$link = 'https://en.wikipedia.org/wiki/68%E2%80%9395%E2%80%9399.7_rule';
				}
				if($method == 'IQR Outlier Test') {
					
					$name = 'IQR Outlier Test (±3 x IQR)';
					$link = 'https://en.wikipedia.org/wiki/Interquartile_range';
				}
				if($method == 'IQR Outlier Test 1.5') {
					
					$checked = '';
					$name = 'IQR Outlier Test (±1.5 x IQR)';
					$link = 'https://en.wikipedia.org/wiki/Interquartile_range';
				}
								
				echo '<div class="form-group form-check form-check-inline">';	
				echo '<input class="form-check-input" type="checkbox" id="checkboxStatisticalMethod_'.$method.'" name="statisticalMethod[]" value="\''.$method.'\'" '.$checked.'>';
				echo '<label class="form-check-label" for="checkboxStatisticalMethod_'.$method.'"> &nbsp;'.$name.' <a href="'.$link.'" target="_blank"><img src="../gfx/info.png" style="width:20px;height:20px;"></a></label>';
				echo '</div> &nbsp; &nbsp;  &nbsp; ';
			}
			
			echo '<h1>Properties to check:</h1> &nbsp;';

			echo '
			<div class="panel-group" id="accordion">
				<div class="panel panel-default" id="panel_global">
					<div class="panel-heading">
						<h4 class="panel-title">
							<a data-toggle="collapse" data-target="#collapse_global" href="#collapseOptions"><b>global</b></a>
						</h4>
					</div>
					<div id="collapse_global" class="panel-collapse collapse">
						<div class="panel-body">';		
														
			
							foreach($propertiesGlobal as $jsonObject) {
				
								$checked = 'checked';
								
								$property = json_decode($jsonObject);
																	
									
								$stringProperty = "";
																	
								$cResultType = $property->cResultType;
								$cIdentifier = $property->cIdentifier;
								$cKey = $property->cKey;				
								
								if($cResultType != 'global') {
									
									$stringProperty .= $cResultType;
									$stringProperty .= ' '.$cIdentifier;
									
									if($cIdentifier > 6) $checked = '';
									if($cIdentifier == 'Ø') $checked = '';
									if($cIdentifier == 'Σ') $checked = '';
								}
								
								if($cKey == 'white matter area') $checked = '';
								if($cKey == 'white matter percentage') $checked = '';
								if($cKey == 'ΔSM') $checked = '';
								
								$stringProperty .= ' '.$cKey;
								
								$stringProperty = trim($stringProperty);
								
								$propertyName = str_replace(" ", "_", $stringProperty);
												
								echo '<div class="form-group form-check form-check">';	
								echo '<input class="form-check-input" type="checkbox" id="checkboxResultProperty_'.$propertyName.'" name="resultProperties[]" value="'.urlencode($jsonObject).'" '.$checked.'>';
								echo '<label class="form-check-label" for="checkboxResultProperty_'.$propertyName.'"> &nbsp;'.$stringProperty.'</label>';
								echo '</div><br/>';
							}
			
			echo '
						</div>
					</div>
				</div>
			</div>';	
			
			echo '
			<div class="panel-group" id="accordion">
				<div class="panel panel-default" id="panel_fissure">
					<div class="panel-heading">
						<h4 class="panel-title">
							<a data-toggle="collapse" data-target="#collapse_fissure" href="#collapseOptions"><b>fissures</b></a>
						</h4>
					</div>
					<div id="collapse_fissure" class="panel-collapse collapse">
						<div class="panel-body">';		
														
			
							foreach($propertiesFissure as $jsonObject) {
				
								$checked = 'checked';
								
								$property = json_decode($jsonObject);
																	
									
								$stringProperty = "";
																	
								$cResultType = $property->cResultType;
								$cIdentifier = $property->cIdentifier;
								$cKey = $property->cKey;				
								
								if($cResultType != 'global') {
									
									$stringProperty .= $cResultType;
									$stringProperty .= ' '.$cIdentifier;
									
									if($cIdentifier > 6) $checked = '';
									if($cIdentifier == 'Ø') $checked = '';
									if($cIdentifier == 'Σ') $checked = '';
								}
								
								if($cKey == 'white matter area') $checked = '';
								if($cKey == 'white matter percentage') $checked = '';
								if($cKey == 'ΔSM') $checked = '';
																					
								$stringProperty .= ' '.$cKey;
								
								$stringProperty = trim($stringProperty);
								
								$propertyName = str_replace(" ", "_", $stringProperty);
												
								echo '<div class="form-group form-check form-check">';	
								echo '<input class="form-check-input" type="checkbox" id="checkboxResultProperty_'.$propertyName.'" name="resultProperties[]" value="'.urlencode($jsonObject).'" '.$checked.'>';
								echo '<label class="form-check-label" for="checkboxResultProperty_'.$propertyName.'"> &nbsp;'.$stringProperty.'</label>';
								echo '</div><br/>';
							}
			
			echo '
						</div>
					</div>
				</div>
			</div>';
			
			echo '
			<div class="panel-group" id="accordion">
				<div class="panel panel-default" id="panel_lobule">
					<div class="panel-heading">
						<h4 class="panel-title">
							<a data-toggle="collapse" data-target="#collapse_lobule" href="#collapseOptions"><b>lobules</b></a>
						</h4>
					</div>
					<div id="collapse_lobule" class="panel-collapse collapse">
						<div class="panel-body">';		
														
			
							foreach($propertiesLobule as $jsonObject) {
				
								$checked = 'checked';
								
								$property = json_decode($jsonObject);
																	
									
								$stringProperty = "";
																	
								$cResultType = $property->cResultType;
								$cIdentifier = $property->cIdentifier;
								$cKey = $property->cKey;				
								
								if($cResultType != 'global') {
									
									$stringProperty .= $cResultType;
									$stringProperty .= ' '.$cIdentifier;
									
									if($cIdentifier > 6) $checked = '';
									if($cIdentifier == 'Ø') $checked = '';
									if($cIdentifier == 'Σ') $checked = '';
								}
								
								if($cKey == 'white matter area') $checked = '';
								if($cKey == 'white matter percentage') $checked = '';
								if($cKey == 'ΔSM') $checked = '';
																					
								$stringProperty .= ' '.$cKey;
								
								$stringProperty = trim($stringProperty);
								
								$propertyName = str_replace(" ", "_", $stringProperty);
												
								echo '<div class="form-group form-check form-check">';	
								echo '<input class="form-check-input" type="checkbox" id="checkboxResultProperty_'.$propertyName.'" name="resultProperties[]" value="'.urlencode($jsonObject).'" '.$checked.'>';
								echo '<label class="form-check-label" for="checkboxResultProperty_'.$propertyName.'"> &nbsp;'.$stringProperty.'</label>';
								echo '</div><br/>';
							}
			
			echo '
						</div>
					</div>
				</div>
			</div>';
			
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