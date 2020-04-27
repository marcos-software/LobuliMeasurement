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

$anomalyArr = array();
$completeSliceCount;

function calculate_median($arr) {
		
	try {
		
		sort($arr);
		$count = count($arr); //total numbers in array
		$middleval = floor(($count-1)/2); // find the middle value, or the lowest middle value
		if($count % 2) { // odd number, middle is the median
			$median = castToFloat($arr[$middleval]);
		} else { // even number, calculate avg of 2 medians
			$low = castToFloat($arr[$middleval]);
			$high = castToFloat($arr[$middleval+1]);
			$median = (($low+$high)/2);
		}
		return $median;
		
	} catch (Exception $e) {
    
		return 1;
	}
}

function castToFloat($string) {
	
	//$number = floatval(str_replace(',', '.', str_replace('.', '', $string)));
	$number = floatval(str_replace(',', '.', $string));
	return $number;
}

function calculate_average($arr) {
	
	try {
	
	sort($arr);
    $count = count($arr); //total numbers in array
    foreach ($arr as $value) {
        $total = $total + castToFloat($value); // total value of array numbers
    }
    $average = ($total/$count); // get average value
    return $average;
	} catch (Exception $e) {
    
		return 1;
	}
}

function calculate_varianz($arr) {
	
	try {
	
	$mean =	calculate_average($arr);
	$result = 0;
	foreach ($arr as $value){
		$result += pow(castToFloat($value) - $mean, 2);
	}
	unset($value);	
    $count = count($arr);	
	$count = ($count == 1) ? $count : $count - 1;
	return ($result / $count);
	} catch (Exception $e) {
    
		return 1;
	}
}

function calculate_standardabweichung($arr) {
	
	try {

    $mean =	calculate_average($arr);
	$result = 0;
	foreach ($arr as $value)
		$result += pow(castToFloat($value) - $mean, 2);
	unset($value);	
    $count = count($arr);	
	$count = ($count == 1) ? $count : $count - 1;
	return sqrt($result / $count);
	} catch (Exception $e) {
    
		return 1;
	}
}

function calculate_standardfehlerMittelwert($arr) {
	
	try {

    $count = count($arr); 
	$standarabweichung = calculate_standardabweichung($arr);	
	return $standarabweichung / sqrt($count);
	} catch (Exception $e) {
    
		return 1;
	}
	
}

function calculate_standardfehlerMedian($arr) {
	
	try {

    $count = count($arr); 
	$standarabweichung = calculate_standardabweichung($arr);
	
	return sqrt(pi() * ($standarabweichung / sqrt($count)));
	} catch (Exception $e) {
    
		return 1;
	}
	
}

function calculate_varianzkoeffizient($arr) {
	
	try {

    $mean = calculate_average($arr);
	$standarabweichung = calculate_standardabweichung($arr);
	
	return ($standarabweichung / $mean);
	} catch (Exception $e) {
    
		return 1;
	}
	
}

function quartile($Array, $Quartile) {
	
	try {
	sort($Array);
  $pos = (count($Array) - 1) * $Quartile;
 
  $base = floor($pos);
  $rest = $pos - $base;
 
  if( isset($Array[$base+1]) ) {
    return castToFloat($Array[$base]) + $rest * (castToFloat($Array[$base+1]) - castToFloat($Array[$base]));
  } else {
    return $Array[$base];
  }
  } catch (Exception $e) {
    
		return 1;
	}
}

function calculate_quartildispersionskoeffizient($arr) {
	
	try {

    $erstesQuartil = quartile($arr, 0.25);
	$drittesQuartil = quartile($arr, 0.75);
	$median = calculate_median($arr);
	
	$result = (($drittesQuartil - $erstesQuartil) / $median);
	
	if($result > 0) return $result;
	return 0.01;
	} catch (Exception $e) {
    
		return 1;
	}
}

function calculate_meanAbsoluteDeviation($arr) 
{  

try {
 sort($arr);
    $mean = calculate_average($arr);
    $n = sizeof($arr);
	
    $absSum = 0; 
    for ($i = 0; $i < $n; $i++) 
        $absSum = $absSum + abs(castToFloat($arr[$i]) - $mean); 
 
    $result = $absSum / $n;	
	if($result > 0) return $result;
	return 0.01;
	} catch (Exception $e) {
    
		return 1;
	}
} 

function calculate_medianAbsoluteDeviation($arr) 
{  

try {
     sort($arr);
    $median = calculate_median($arr);
	
	$d = array();
	
	foreach ($arr as $value){
			
		array_push ( $d , abs(castToFloat($value) - $median));
	}
	
	$result =  calculate_median($d);
	if($result > 0) return $result;
	return 0.01;
	} catch (Exception $e) {
    
		return 1;
	}
	
} 

function normalize($val)
{
	$CONST_NORMALIZE_TRESHOLD = 200;
	
	$result = floatval($val);	
	
	try 
	{
		
		if($result > $CONST_NORMALIZE_TRESHOLD) $result = $CONST_NORMALIZE_TRESHOLD;
		if($result < ($CONST_NORMALIZE_TRESHOLD * -1)) $result = ($CONST_NORMALIZE_TRESHOLD * -1);	
		 
	} catch (Exception $e) {
		$result = $val;
	}	
	
	return $result;
}


function generateData($statisticalMethod, 
$delta_mittelwert_normalized, 
$delta_varianz_normalized, 
$delta_standardabweichung_normalized, 
$delta_standardfehlerMittelwert_normalized, 
$delta_standardfehlerMedian_normalized, 
$delta_varianzkoeffizienz_normalized,
$delta_quartildispersionskoeffizient_normalized,
$delta_minimum_normalized,
$delta_maximum_normalized,
$delta_median_normalized,
$delta_spannweite_normalized,
$delta_mittlereAbweichung_normalized,
$delta_medianAbweichung_normalized
)
{		
	$result = '';
	
	if(in_array('Mittelwert', $statisticalMethod)) $result .= $delta_mittelwert_normalized.',';
	if(in_array('Varianz', $statisticalMethod)) $result .= $delta_varianz_normalized.',';
	if(in_array('Standard Abweichung', $statisticalMethod)) $result .= $delta_standardabweichung_normalized.',';
	if(in_array('Standard Fehler Mittelwert', $statisticalMethod)) $result .= $delta_standardfehlerMittelwert_normalized.',';
	if(in_array('Standard Fehler Median', $statisticalMethod)) $result .= $delta_standardfehlerMedian_normalized.',';
	if(in_array('Varianz Koeffizient', $statisticalMethod)) $result .= $delta_varianzkoeffizienz_normalized.',';
	if(in_array('Quartil Dispersionskoeffizient', $statisticalMethod)) $result .= $delta_quartildispersionskoeffizient_normalized.',';
	if(in_array('Minimum', $statisticalMethod)) $result .= $delta_minimum_normalized.',';
	if(in_array('Maximum', $statisticalMethod)) $result .= $delta_maximum_normalized.',';
	if(in_array('Median', $statisticalMethod)) $result .= $delta_median_normalized.',';
	if(in_array('Spannweite', $statisticalMethod)) $result .= $delta_spannweite_normalized.',';
	if(in_array('Mittlere Abweichung', $statisticalMethod)) $result .= $delta_mittlereAbweichung_normalized.',';
	if(in_array('Median Abweichung', $statisticalMethod)) $result .= $delta_medianAbweichung_normalized.',';
	
	$result = rtrim($result, ',');
	
	return $result;
}

function generateDataRadarDiagram($statisticalMethod, 
$delta_mittelwert, 
$delta_varianz, 
$delta_standardabweichung, 
$delta_standardfehlerMittelwert, 
$delta_standardfehlerMedian, 
$delta_varianzkoeffizienz,
$delta_quartildispersionskoeffizient,
$delta_minimum,
$delta_maximum,
$delta_median,
$delta_spannweite,
$delta_mittlereAbweichung,
$delta_medianAbweichung
)
{		
	$result = '';
	
	if(in_array('Mittelwert', $statisticalMethod)) $result .= abs($delta_mittelwert).',';
	if(in_array('Varianz', $statisticalMethod)) $result .= abs($delta_varianz).',';
	if(in_array('Standard Abweichung', $statisticalMethod)) $result .= abs($delta_standardabweichung).',';
	if(in_array('Standard Fehler Mittelwert', $statisticalMethod)) $result .= abs($delta_standardfehlerMittelwert).',';
	if(in_array('Standard Fehler Median', $statisticalMethod)) $result .= abs($delta_standardfehlerMedian).',';
	if(in_array('Varianz Koeffizient', $statisticalMethod)) $result .= abs($delta_varianzkoeffizienz).',';
	if(in_array('Quartil Dispersionskoeffizient', $statisticalMethod)) $result .= abs($delta_quartildispersionskoeffizient).',';
	if(in_array('Minimum', $statisticalMethod)) $result .= abs($delta_minimum).',';
	if(in_array('Maximum', $statisticalMethod)) $result .= abs($delta_maximum).',';
	if(in_array('Median', $statisticalMethod)) $result .= abs($delta_median).',';
	if(in_array('Spannweite', $statisticalMethod)) $result .= abs($delta_spannweite).',';
	if(in_array('Mittlere Abweichung', $statisticalMethod)) $result .= abs($delta_mittlereAbweichung).',';
	if(in_array('Median Abweichung', $statisticalMethod)) $result .= abs($delta_medianAbweichung).',';
	
	$result = rtrim($result, ',');
	
	return $result;
}

function generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod)
{		
	        require_once('db.php');
		    $pdoLog     = getPDO(1);
	
	        $key = $vCut['cKey'];
			$key_underscore = str_replace(" ", "_", $key);
						
			$queryWT = 'SELECT cValue '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cKey = '".$key."' AND vCut.cGenotype = 'WT'";
			$queryKO = 'SELECT cValue '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cKey = '".$key."' AND vCut.cGenotype = 'KO'";
			
			if($cResultType != 'global')
			{
				$queryWT .= " AND vResult.cIdentifier = '$cIdentifier'";;		
				$queryKO .= " AND vResult.cIdentifier = '$cIdentifier'";;				
			}
			
			$statement  = $pdoLog->prepare($queryWT);
			$statement->execute();
			$vCutWTList    = $statement->fetchAll();
			
			$statement  = $pdoLog->prepare($queryKO);
			$statement->execute();
			$vCutKOList    = $statement->fetchAll();
			
			$valuesWT = array();
			$valuesKO = array();
						
			foreach($vCutWTList as $keyWT => $vCutWT) {
				
				array_push($valuesWT, castToFloat($vCutWT['cValue']));
			}
			
			foreach($vCutKOList as $keyKO => $vCutKO) {
				
				array_push($valuesKO, castToFloat($vCutKO['cValue']));
			}
			
			#region 3 sigma regel
			$mittelwert_wt = number_format(calculate_average($valuesWT), 2 , '.', '');
			$mittelwert_ko = number_format(calculate_average($valuesKO), 2 , '.', '');
			
			$mittlereAbweichung_wt = number_format(calculate_meanAbsoluteDeviation($valuesWT), 2 , '.', '');
			$mittlereAbweichung_ko = number_format(calculate_meanAbsoluteDeviation($valuesKO), 2 , '.', '');
			#endregion 3 sigma regel
			
			#region iqr outlier test
			$erstesQuartil_wt = quartile($valuesWT, 0.25);
			$drittesQuartil_wt = quartile($valuesWT, 0.75);	

			$erstesQuartil_ko = quartile($valuesKO, 0.25);
			$drittesQuartil_ko = quartile($valuesKO, 0.75);		
			
			$iqr_wt = $drittesQuartil_wt - $erstesQuartil_wt;
			$iqr_ko = $drittesQuartil_ko - $erstesQuartil_ko;			
			#endregion iqr outlier test		
			
			global $methodenWerte;
			
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittelwert_wt'] = $mittelwert_wt;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittelwert_ko'] = $mittelwert_ko;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittlereAbweichung_wt'] = $mittlereAbweichung_wt;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittlereAbweichung_ko'] = $mittlereAbweichung_ko;
			
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['erstesQuartil_wt'] = $erstesQuartil_wt;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['drittesQuartil_wt'] = $drittesQuartil_wt;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['erstesQuartil_ko'] = $erstesQuartil_ko;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['drittesQuartil_ko'] = $drittesQuartil_ko;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['iqr_wt'] = $iqr_wt;
			$methodenWerte[$cResultType][$cIdentifier][$key_underscore]['iqr_ko'] = $iqr_ko;
}

function checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties)
{		

		$key = $vCut['cKey'];
		$key_underscore = str_replace(" ", "_", $key);		
		
		if($resultProperties[$cResultType][$cIdentifier] == null || in_array($key, $resultProperties[$cResultType][$cIdentifier]) === false) {
			
			return;
		}	
		
		global $anomalyArr;
	
		require_once('db.php');
		$pdoLog     = getPDO(1);		
					
		$queryWT = 'SELECT vResult.kCut, vResult.cValue, vResult.cUnit, vCut.kAge, vCut.cGenotype, vCut.cAnimal, vCut.cCutIdentifier '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cKey = '".$key."' AND vCut.cGenotype = 'WT'";
		$queryKO = 'SELECT vResult.kCut, vResult.cValue, vResult.cUnit, vCut.kAge, vCut.cGenotype, vCut.cAnimal, vCut.cCutIdentifier '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cKey = '".$key."' AND vCut.cGenotype = 'KO'";
		
		if($cResultType != 'global')
		{
			$queryWT .= " AND vResult.cIdentifier = '$cIdentifier'";;		
			$queryKO .= " AND vResult.cIdentifier = '$cIdentifier'";;				
		}
		
		$queryWT .= 'ORDER BY vCut.kAge, vCut.cGenotype, vCut.cAnimal, vCut.cCutIdentifier';
		$queryKO .= 'ORDER BY vCut.kAge, vCut.cGenotype, vCut.cAnimal, vCut.cCutIdentifier';
		
		$statement  = $pdoLog->prepare($queryWT);
		$statement->execute();
		$vCutWTList    = $statement->fetchAll();
		
		$statement  = $pdoLog->prepare($queryKO);
		$statement->execute();
		$vCutKOList    = $statement->fetchAll();
		
		$valuesWT = array();
		$valuesKO = array();	

		global $methodenWerte;
		global $anomalies;
		global $anomaliesCounter;
		
		$mittelwert_wt = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittelwert_wt'];
		$mittelwert_ko = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittelwert_ko'];
		$mittlereAbweichung_wt = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittlereAbweichung_wt'];
		$mittlereAbweichung_ko = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['mittlereAbweichung_ko'];
			
		$erstesQuartil_wt = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['erstesQuartil_wt'];
		$drittesQuartil_wt = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['drittesQuartil_wt'];
		$erstesQuartil_ko = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['erstesQuartil_ko'];
		$drittesQuartil_ko = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['drittesQuartil_ko'];
		$iqr_wt = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['iqr_wt'];
		$iqr_ko = $methodenWerte[$cResultType][$cIdentifier][$key_underscore]['iqr_ko'];
					
		foreach($vCutWTList as $keyWT => $vCutWT) {
		
			$cValue = castToFloat($vCutWT['cValue']);
			$kCut = $vCutWT['kCut'];
			
			#region three sigma rule
			if(in_array('Three Sigma Rule', $statisticalMethod)) {
				
				if(abs($cValue - $mittelwert_wt) > (3 * $mittlereAbweichung_wt)) {
					
					if($anomalies['Three Sigma Rule'][$kCut] == null){
						
						$anomalies['Three Sigma Rule'][$kCut] = array();
					}
					
					$limitValue = $cValue;
					$limitMin = -1 * ((3 * $mittlereAbweichung_wt) - $mittelwert_wt);
					$limitMax =  1 * ((3 * $mittlereAbweichung_wt) + $mittelwert_wt);
					
					if($limitValue < 0)	$limitValue = 0;
					if($limitMin < 0)	$limitMin = 0;
					if($limitMax < 0)	$limitMax = 0;			
					
					$limitValueString = number_format($limitValue, 2 , '.', '');
					$limitMinString = number_format($limitMin, 2 , '.', '');
					$limitMaxString = number_format($limitMax, 2 , '.', '');
					$limitUnit = $vCutWT['cUnit'];;
					
					$limitResult = "<font size=\"1\"><i> &nbsp; &nbsp; Value: $limitValueString $limitUnit (Limit Min: $limitMinString $limitUnit | Max: $limitMaxString $limitUnit)</i></font>";
					
					$anomaly = new stdClass();
					$anomaly->cResultType = $cResultType;
					$anomaly->cIdentifier = $cIdentifier;
					$anomaly->cKey = $key;
					$anomaly->limitResult = $limitResult;
										
					array_push($anomalies['Three Sigma Rule'][$kCut], json_encode($anomaly));
					
					$anomalyArrEntryString = $vCutWT['kAge'].$vCutWT['cGenotype'].$vCutWT['cAnimal'].$vCutWT['cCutIdentifier'];					
					
					if(in_array($anomalyArrEntryString, $anomalyArr) == false) {
					
						array_push($anomalyArr, $anomalyArrEntryString);
					}
					
				}
			}			
			#endregion
			
			#region iqr outlier test 3 IQR	
			if(in_array('IQR Outlier Test', $statisticalMethod)) {	
			
				if(
				
				 ($cValue < ($erstesQuartil_wt - (3.0 * $iqr_wt)))
				 ||
				 (($drittesQuartil_wt + (3.0 * $iqr_wt)) < $cValue)
				
				) {
					
					if($anomalies['IQR Outlier Test'][$kCut] == null){
						
						$anomalies['IQR Outlier Test'][$kCut] = array();
					}
					
					
					
					$limitValue = $cValue;
					$limitMin = ($erstesQuartil_wt - (3.0 * $iqr_wt));
					$limitMax = ($drittesQuartil_wt + (3.0 * $iqr_wt));
					
					if($limitValue < 0)	$limitValue = 0;
					if($limitMin < 0)	$limitMin = 0;
					if($limitMax < 0)	$limitMax = 0;			
					
					$limitValueString = number_format($limitValue, 2 , '.', '');
					$limitMinString = number_format($limitMin, 2 , '.', '');
					$limitMaxString = number_format($limitMax, 2 , '.', '');
					$limitUnit = $vCutWT['cUnit'];;
					
					$limitResult = "<font size=\"1\"><i> &nbsp; &nbsp; Value: $limitValueString $limitUnit (Limit Min: $limitMinString $limitUnit | Max: $limitMaxString $limitUnit)</i></font>";
													
					$anomaly = new stdClass();
					$anomaly->cResultType = $cResultType;
					$anomaly->cIdentifier = $cIdentifier;
					$anomaly->cKey = $key;
					$anomaly->limitResult = $limitResult;
										
					array_push($anomalies['IQR Outlier Test'][$kCut], json_encode($anomaly));	
					
					$anomalyArrEntryString = $vCutWT['kAge'].$vCutWT['cGenotype'].$vCutWT['cAnimal'].$vCutWT['cCutIdentifier'];					
					if(in_array($anomalyArrEntryString, $anomalyArr) == false) {
					
						array_push($anomalyArr, $anomalyArrEntryString);
					}
				}	
			}			
			#endregion
			
			#region iqr outlier test 1.5 IQR		
			if(in_array('IQR Outlier Test 1.5', $statisticalMethod)) {	
			
				if(
				
				 ($cValue < ($erstesQuartil_wt - (1.5 * $iqr_wt)))
				 ||
				 (($drittesQuartil_wt + (1.5 * $iqr_wt)) < $cValue)
				
				) {
					
					if($anomalies['IQR Outlier Test 1.5'][$kCut] == null){
						
						$anomalies['IQR Outlier Test 1.5'][$kCut] = array();
					}
					
					$limitValue = $cValue;
					$limitMin = ($erstesQuartil_wt - (1.5 * $iqr_wt));
					$limitMax = ($drittesQuartil_wt + (1.5 * $iqr_wt));
					
					if($limitValue < 0)	$limitValue = 0;
					if($limitMin < 0)	$limitMin = 0;
					if($limitMax < 0)	$limitMax = 0;			
					
					$limitValueString = number_format($limitValue, 2 , '.', '');
					$limitMinString = number_format($limitMin, 2 , '.', '');
					$limitMaxString = number_format($limitMax, 2 , '.', '');
					$limitUnit = $vCutWT['cUnit'];;
					
					$limitResult = "<font size=\"1\"><i> &nbsp; &nbsp; Value: $limitValueString $limitUnit (Limit Min: $limitMinString $limitUnit | Max: $limitMaxString $limitUnit)</i></font>";
					
					$anomaly = new stdClass();
					$anomaly->cResultType = $cResultType;
					$anomaly->cIdentifier = $cIdentifier;
					$anomaly->cKey = $key;
					$anomaly->limitResult = $limitResult;
										
					array_push($anomalies['IQR Outlier Test 1.5'][$kCut], json_encode($anomaly));
					
					$anomalyArrEntryString = $vCutWT['kAge'].$vCutWT['cGenotype'].$vCutWT['cAnimal'].$vCutWT['cCutIdentifier'];
					if(in_array($anomalyArrEntryString, $anomalyArr) == false) {
					
						array_push($anomalyArr, $anomalyArrEntryString);
					}					
				}	
			}			
			#endregion
			
			if($anomal) $anomalyCount++;
		}
		
		foreach($vCutKOList as $keyKO => $vCutKO) {
			
			$cValue = castToFloat($vCutKO['cValue']);
			$kCut = $vCutKO['kCut'];
			$anomal = false;		
			
			#region three sigma rule
			if(in_array('Three Sigma Rule', $statisticalMethod)) {
				
				if(abs($cValue - $mittelwert_ko) > (3 * $mittlereAbweichung_ko)) {
					
					if($anomalies['Three Sigma Rule'][$kCut] == null){
						
						$anomalies['Three Sigma Rule'][$kCut] = array();
					}
					
					$limitValue = $cValue;
					$limitMin = -1 * ((3 * $mittlereAbweichung_ko) - $mittelwert_ko);
					$limitMax =  1 * ((3 * $mittlereAbweichung_ko) + $mittelwert_ko);	

					if($limitValue < 0)	$limitValue = 0;
					if($limitMin < 0)	$limitMin = 0;
					if($limitMax < 0)	$limitMax = 0;					
								
					$limitValueString = number_format($limitValue, 2 , '.', '');
					$limitMinString = number_format($limitMin, 2 , '.', '');
					$limitMaxString = number_format($limitMax, 2 , '.', '');
					$limitUnit = $vCutKO['cUnit'];;
					
					$limitResult = "<font size=\"1\"><i> &nbsp; &nbsp; Value: $limitValueString $limitUnit (Limit Min: $limitMinString $limitUnit | Max: $limitMaxString $limitUnit)</i></font>";
					
					$anomaly = new stdClass();
					$anomaly->cResultType = $cResultType;
					$anomaly->cIdentifier = $cIdentifier;
					$anomaly->cKey = $key;
					$anomaly->limitResult = $limitResult;
					
					array_push($anomalies['Three Sigma Rule'][$kCut], json_encode($anomaly));	
					
					$anomalyArrEntryString = $vCutKO['kAge'].$vCutKO['cGenotype'].$vCutKO['cAnimal'].$vCutKO['cCutIdentifier'];
					if(in_array($anomalyArrEntryString, $anomalyArr) == false) {
					
						array_push($anomalyArr, $anomalyArrEntryString);
					}
				}
			}			
			#endregion
			
			#region iqr outlier test 3.0		
			if(in_array('IQR Outlier Test', $statisticalMethod)) {	
			
				if(
				
				 ($cValue < ($erstesQuartil_ko - (3.0 * $iqr_ko)))
				 ||
				 (($drittesQuartil_ko + (3.0 * $iqr_ko)) < $cValue)
				
				) {
					
					if($anomalies['IQR Outlier Test'][$kCut] == null){
						
						$anomalies['IQR Outlier Test'][$kCut] = array();
					}
					
					$limitValue = $cValue;
					$limitMin = ($erstesQuartil_ko - (3.0 * $iqr_ko));
					$limitMax = ($drittesQuartil_ko + (3.0 * $iqr_ko));
					
					if($limitValue < 0)	$limitValue = 0;
					if($limitMin < 0)	$limitMin = 0;
					if($limitMax < 0)	$limitMax = 0;			
					
					$limitValueString = number_format($limitValue, 2 , '.', '');
					$limitMinString = number_format($limitMin, 2 , '.', '');
					$limitMaxString = number_format($limitMax, 2 , '.', '');
					$limitUnit = $vCutKO['cUnit'];;
					
					$limitResult = "<font size=\"1\"><i> &nbsp; &nbsp; Value: $limitValueString $limitUnit (Limit Min: $limitMinString $limitUnit | Max: $limitMaxString $limitUnit)</i></font>";
					
					$anomaly = new stdClass();
					$anomaly->cResultType = $cResultType;
					$anomaly->cIdentifier = $cIdentifier;
					$anomaly->cKey = $key;
					$anomaly->limitResult = $limitResult;
					
					array_push($anomalies['IQR Outlier Test'][$kCut], json_encode($anomaly));	
					
					$anomalyArrEntryString = $vCutKO['kAge'].$vCutKO['cGenotype'].$vCutKO['cAnimal'].$vCutKO['cCutIdentifier'];
					if(in_array($anomalyArrEntryString, $anomalyArr) == false) {
					
						array_push($anomalyArr, $anomalyArrEntryString);
					}
				}	
			}			
			#endregion
			
			#region iqr outlier test 1.5	
			if(in_array('IQR Outlier Test 1.5', $statisticalMethod)) {	
			
				if(
				
				 ($cValue < ($erstesQuartil_ko - (1.5 * $iqr_ko)))
				 ||
				 (($drittesQuartil_ko + (1.5 * $iqr_ko)) < $cValue)
				
				) {
					
					if($anomalies['IQR Outlier Test 1.5'][$kCut] == null){
						
						$anomalies['IQR Outlier Test 1.5'][$kCut] = array();
					}
					
					$limitValue = $cValue;
					$limitMin = ($erstesQuartil_ko - (1.5 * $iqr_ko));
					$limitMax = ($drittesQuartil_ko + (1.5 * $iqr_ko));
					
					if($limitValue < 0)	$limitValue = 0;
					if($limitMin < 0)	$limitMin = 0;
					if($limitMax < 0)	$limitMax = 0;			
					
					$limitValueString = number_format($limitValue, 2 , '.', '');
					$limitMinString = number_format($limitMin, 2 , '.', '');
					$limitMaxString = number_format($limitMax, 2 , '.', '');
					$limitUnit = $vCutKO['cUnit'];;
					
					$limitResult = "<font size=\"1\"><i> &nbsp; &nbsp; Value: $limitValueString $limitUnit (Limit Min: $limitMinString $limitUnit | Max: $limitMaxString $limitUnit)</i></font>";
					
					$anomaly = new stdClass();
					$anomaly->cResultType = $cResultType;
					$anomaly->cIdentifier = $cIdentifier;
					$anomaly->cKey = $key;
					$anomaly->limitResult = $limitResult;
					
					array_push($anomalies['IQR Outlier Test 1.5'][$kCut], json_encode($anomaly));	
					
					$anomalyArrEntryString = $vCutKO['kAge'].$vCutKO['cGenotype'].$vCutKO['cAnimal'].$vCutKO['cCutIdentifier'];
					if(in_array($anomalyArrEntryString, $anomalyArr) == false) {
					
						array_push($anomalyArr, $anomalyArrEntryString);
					}
				}	
			}			
			#endregion
			
			if($anomal) $anomalyCount++;
		}
}

function compareArraySizeNegatively($valueA, $valueB){
	
	if(is_array($valueA) == false) return 1;
	if(is_array($valueB) == false) return -1;
	
    return sizeof($valueB)-sizeof($valueA);
}

function compareArrayValueNegativly($valueA, $valueB){
		
    return $valueB-$valueA;
}



	$ageFrom           = $_POST['ageFrom'];
	$ageTo             = $_POST['ageTo'];
	$animals           = $_POST['animals'];
	$layer             = $_POST['layer'];
	$statisticalMethod = $_POST['statisticalMethod'];
	$resultPropertiesRaw = $_POST['resultProperties'];
	
    readfile("../html/head.html");	
	
	if
	(
		(is_null($ageFrom) == false && isset($ageFrom) && trim($ageFrom) !== '') &&
		(is_null($ageTo) == false && isset($ageTo) && trim($ageTo) !== '') &&
		(is_null($animals) == false && isset($animals)) &&
		(is_null($layer) == false && isset($layer)) &&
		(is_null($statisticalMethod) == false && isset($statisticalMethod)) &&
		(is_null($resultPropertiesRaw) == false && isset($resultPropertiesRaw))
	)	
	{		
	
	$statisticalMethodStringed = $statisticalMethod;
	
	foreach($statisticalMethod as $key=>$value){
	  $statisticalMethod[$key]=str_replace("'",'',$value);
	}	
	
	$resultProperties = array();
	$resultPropertiesString = array();	
	
	foreach($resultPropertiesRaw as $jsonObject){
	  
		$resultProperty = json_decode(urldecode($jsonObject));
				
		$cResultType = $resultProperty->cResultType;
		$cIdentifier = $resultProperty->cIdentifier;
		$cKey = $resultProperty->cKey;
		
		if($resultProperties[$cResultType] == null){
			
			$resultProperties[$cResultType] = array();
		}
		
		if($resultProperties[$cResultType][$cIdentifier] == null){
			
			$resultProperties[$cResultType][$cIdentifier] = array();
		}
		
		array_push($resultProperties[$cResultType][$cIdentifier], $cKey);
		
		$stringProperty = "";												
		
		if($cResultType != 'global') {
			
			$stringProperty .= $cResultType;
			$stringProperty .= ' '.$cIdentifier;
		}
				
		$stringProperty .= ' '.$cKey;		
		$stringProperty = trim($stringProperty);
								
		array_push($resultPropertiesString, $stringProperty);		
	}

	$query = 'FROM vResult JOIN vCut ON vCut.kCut = vResult.kCut';
	
	$query .= ' WHERE ';
	if($ageFrom <= $ageTo) {
		
		$query .= 'vCut.kAge >= '.$ageFrom.' and vCut.kAge <= '.$ageTo;
	} else {
		
		$query .= 'vCut.kAge >= '.$ageTo.' and vCut.kAge <= '.$ageFrom;
	}	
	
	$query .= ' AND vCut.bExcludeFromAnalysis = 0 ';
	
	$query .= ' AND vCut.cAnimal IN (';
	$query .= join(',', $animals);	
	$query .= ')';
	
	$query .= ' AND vCut.cLayer IN (';
	$query .= join(',', $layer);	
	$query .= ')';
	
	echo '<div class="container">';	
	    echo '<br/>';
		echo '<a href="anomalies.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Specify Filters</button></a>';
		echo '<h1>Anomalies</h1><br/>';
		echo '<h3>Specified Filters:</h3>';
		echo '<h5>&emsp;Age from '.$ageFrom.' to '.$ageTo.'</h5>';
		echo '<h5>&emsp;Animals: '.str_replace("'", '', join(', ', $animals)).'</h5>';
		echo '<h5>&emsp;Layer: '.str_replace("'", '', join(', ', $layer)).'</h5>';		
				
		require_once('db.php');
		$pdoLog     = getPDO(1);	

		$methodenWerte = array();
		$methodenWerte['global'] = array();
		$methodenWerte['fissure'] = array();
		$methodenWerte['lobule'] = array();
		
		$anomalies = array();
		
		foreach($statisticalMethod as $key=>$value) {		  
		
		  $anomalies[$value] = array();
		}		
		

		$statement  = $pdoLog->prepare('SELECT COUNT(DISTINCT vCut.kCut) AS kCompleteSliceCount '.$query."");
		$statement->execute();
		$vCutCount    = $statement->fetch();
	
		$completeSliceCount = intval($vCutCount['kCompleteSliceCount']);
		
		#region GenerateMethodenWerte
			#region global
			$cResultType = 'global';
			$cIdentifier = '';											
			
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod);			
			}
			#endregion global
			
			#region fissure	
			$cResultType = 'fissure';
			
			$cIdentifier = 'Ø';		
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod);					
			}
			
			$cIdentifier = 'Σ';
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod);					
			}
			
			for($i = 1; $i <= 10; $i++) {
				
				$cIdentifier = $i;
				
				$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
				$statement->execute();
				$vCutKeysGlobal    = $statement->fetchAll();

				foreach($vCutKeysGlobal as $key => $vCut) {
					
					generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod);					
				}
			}
			#endregion fissure	
			
			#region lobule	
			$cResultType = 'lobule';
			
			$cIdentifier = 'Ø';		
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
			}
			
			$cIdentifier = 'Σ';
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
			}
			
			for($i = 1; $i <= 10; $i++) {
				
				$cIdentifier = $i;
				
				$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
				$statement->execute();
				$vCutKeysGlobal    = $statement->fetchAll();

				foreach($vCutKeysGlobal as $key => $vCut) {
					
					generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
				}
			}
			#endregion lobule	
		#endregion GenerateMethodenWerte
		
		#region AnalyseAnomlies
		
			#region global
			$cResultType = 'global';
			$cIdentifier = '';	
			
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey '.$query." AND vResult.cResultType = '".$cResultType."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);			
			}			
			#endregion global
			
			#region fissure	
			$cResultType = 'fissure';
			
			$cIdentifier = 'Ø';		
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);				
			}
			
			$cIdentifier = 'Σ';
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);				
			}
			
			for($i = 1; $i <= 10; $i++) {
				
				$cIdentifier = $i;
				
				$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
				$statement->execute();
				$vCutKeysGlobal    = $statement->fetchAll();

				foreach($vCutKeysGlobal as $key => $vCut) {
					
					checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);					
				}
			}
			#endregion fissure	
			
			#region lobule	
			$cResultType = 'lobule';
			
			$cIdentifier = 'Ø';		
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);				
			}
			
			$cIdentifier = 'Σ';
			$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
			$statement->execute();
			$vCutKeysGlobal    = $statement->fetchAll();

			foreach($vCutKeysGlobal as $key => $vCut) {
				
				checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);				
			}
			
			for($i = 1; $i <= 10; $i++) {
				
				$cIdentifier = $i;
				
				$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
				$statement->execute();
				$vCutKeysGlobal    = $statement->fetchAll();

				foreach($vCutKeysGlobal as $key => $vCut) {
					
					checkDifferences($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $resultProperties);					
				}
			}
			#endregion lobule	
		
		#endregion AnalyseAnomalies

		#region print Output
		
		echo '<h5>&emsp;Properties to check: <font size="1">'.str_replace("'", '', join(', ', $resultPropertiesString)).'</font></h5>';
		if(count($anomalyArr) == 0 || $completeSliceCount == 0){
			
			echo '<h5>&emsp;Quality: 100 % (analyzed slices: '.$completeSliceCount.' | anomal slices: '.count($anomalyArr).')</h5>';				
		}else{
			
			echo '<h5>&emsp;<b>Quality: '.number_format((100 * (1 - (count($anomalyArr) / $completeSliceCount))), 2 , '.', '').' % (analyzed slices: '.$completeSliceCount.' | anomal slices: '.count($anomalyArr).')</b></h5>';	
		}
		echo '<br/>';	
		
		if(in_array('Three Sigma Rule', $statisticalMethod)) {
			
			echo '
			<div class="panel-group" id="accordion">
				<div class="panel panel-default" id="panel_Three_Sigma_Rule">
					<div class="panel-heading">
						<h4 class="panel-title">
							<a data-toggle="collapse" data-target="#collapse_Three_Sigma_Rule" href="#collapseOptions1"><b>Three Sigma Rule</b> <font size="2">('.count($anomalies['Three Sigma Rule']).' anomal slices detected)</font></a>
						</h4>
					</div>
					<div id="collapse_Three_Sigma_Rule" class="panel-collapse collapse">
						<div class="panel-body">
							<table class="table table-striped">
								<tbody>';	

								readfile("../html/anomaliesTableHead.html");		

								$tempAnomalies = array();
								$tempAnomalies = $anomalies['Three Sigma Rule'];
								uasort($tempAnomalies,'compareArraySizeNegatively');	

								if($anomaliesCounter['Three Sigma Rule'] == null){
						
									$anomaliesCounter['Three Sigma Rule'] = array();
								}									
														
										foreach($tempAnomalies as $key => $value) {
											
											$statement  = $pdoLog->prepare('SELECT * FROM vCut WHERE vCut.kCut = '.$key);
											$statement->execute();
											$vCut    = $statement->fetch();
											
											echo '<tr role="row">';			
												echo '<td>'.$vCut['kAge'].'</td>';
												echo '<td>'.$vCut['cGenotype'].'</td>';
												echo '<td>'.$vCut['cAnimal'].'</td>';
												echo '<td>'.$vCut['cCutIdentifier'].'</td>';				
												echo '<td><ul><font size="2">';
												
												$stringAnomaly = "";
												
												foreach($value as $key2 => $jsonObject) {
													
													$anomaly = json_decode($jsonObject);
													
													$cResultType = $anomaly->cResultType;
													$cIdentifier = $anomaly->cIdentifier;
													$cKey = $anomaly->cKey;	
													$limitResult = $anomaly->limitResult;													
													
													$stringAnomaly .= '<li><b>';
													
													$anomalyKeyIdentifier = '';
													
													if($cResultType != 'global') {
														
														$anomalyKeyIdentifier .= $cResultType;
														$anomalyKeyIdentifier .= ' '.$cIdentifier;
													}
													
													$anomalyKeyIdentifier .= ' '.$cKey;
													
													$stringAnomaly .= $anomalyKeyIdentifier;
													
													
													$stringAnomaly .= '</b> '.$limitResult;
													$stringAnomaly .= '</li>';	

													if($anomaliesCounter['Three Sigma Rule'][$anomalyKeyIdentifier] == null){
														
														$anomaliesCounter['Three Sigma Rule'][$anomalyKeyIdentifier] = 1;
												
													}
													else
													{
														$anomaliesCounter['Three Sigma Rule'][$anomalyKeyIdentifier] = $anomaliesCounter['Three Sigma Rule'][$anomalyKeyIdentifier] + 1;
													}												
													
												}
												
												echo $stringAnomaly;
												
												echo'</font></ul></td>';
												echo '<td><a target="_blank" href="details.php?age='.$vCut['kAge'].'&genotype='.$vCut['cGenotype'].'&animal='.$vCut['cAnimal'].'&cutidentifier='.$vCut['cCutIdentifier'].'"><i class="fa fa-pencil"><button type="button" class="btn btn-default">Show Details</button></i></a></td>';
											echo '</tr>';	
										}						
						
			echo '
								</tbody>
							</table>
						</div>
					</div>';				
				
					$tempAnomalies = array();
					$tempAnomalies = $anomaliesCounter['Three Sigma Rule'];
					uasort($tempAnomalies,'compareArrayValueNegativly');
					echo '&emsp;<b>Properties with most anomalies:</b>';
					echo '&emsp;<ul><font size="2">';
					foreach($tempAnomalies as $key => $value) {
						
						echo '<li>'.$key.': '.$value.' x</li>';
						
					}
					echo '</font></ul>	
				</div>				
			</div>';
		}
		
		if(in_array('IQR Outlier Test', $statisticalMethod)) {
			
			echo '
			<div class="panel-group" id="accordion">
				<div class="panel panel-default" id="panel_IQR_Outlier_Test">
					<div class="panel-heading">
						<h4 class="panel-title">
							<a data-toggle="collapse" data-target="#collapse_IQR_Outlier_Test" href="#collapseOptions2"><b>IQR Outlier Test (±3 x IQR)</b> <font size="2">('.count($anomalies['IQR Outlier Test']).' anomal slices detected) [Click here for details]</font></a>
						</h4>
					</div>
					<div id="collapse_IQR_Outlier_Test" class="panel-collapse collapse">
						<div class="panel-body">
							<table class="table table-striped">
								<tbody>';		
								
								readfile("../html/anomaliesTableHead.html");
								
								$tempAnomalies = array();
								$tempAnomalies = $anomalies['IQR Outlier Test'];
								uasort($tempAnomalies,'compareArraySizeNegatively');	

								if($anomaliesCounter['IQR Outlier Test'] == null){
						
									$anomaliesCounter['IQR Outlier Test'] = array();
								}								
								
										foreach($tempAnomalies as $key => $value) {
											
											$statement  = $pdoLog->prepare('SELECT * FROM vCut WHERE vCut.kCut = '.$key);
											$statement->execute();
											$vCut    = $statement->fetch();		
											
											echo '<tr role="row">';			
												echo '<td>'.$vCut['kAge'].'</td>';
												echo '<td>'.$vCut['cGenotype'].'</td>';
												echo '<td>'.$vCut['cAnimal'].'</td>';
												echo '<td>'.$vCut['cCutIdentifier'].'</td>';				
												echo '<td><ul><font size="2">';
												
												$stringAnomaly = "";
												
												foreach($value as $key2 => $jsonObject) {
													
													$anomaly = json_decode($jsonObject);
													
													$cResultType = $anomaly->cResultType;
													$cIdentifier = $anomaly->cIdentifier;
													$cKey = $anomaly->cKey;	
													$limitResult = $anomaly->limitResult;													
													
													$stringAnomaly .= '<li><b>';
													
													$anomalyKeyIdentifier = '';
													
													if($cResultType != 'global') {
														
														$anomalyKeyIdentifier .= $cResultType;
														$anomalyKeyIdentifier .= ' '.$cIdentifier;
													}
													
													$anomalyKeyIdentifier .= ' '.$cKey;
													
													$stringAnomaly .= $anomalyKeyIdentifier;
													
													
													$stringAnomaly .= '</b> '.$limitResult;
													$stringAnomaly .= '</li>';	

													if($anomaliesCounter['IQR Outlier Test'][$anomalyKeyIdentifier] == null){
														
														$anomaliesCounter['IQR Outlier Test'][$anomalyKeyIdentifier] = 1;
												
													}
													else
													{
														$anomaliesCounter['IQR Outlier Test'][$anomalyKeyIdentifier] = $anomaliesCounter['IQR Outlier Test'][$anomalyKeyIdentifier] + 1;
													}												
													
												}
												
												echo $stringAnomaly;
												
												echo'</font></ul></td>';
												echo '<td><a target="_blank" href="details.php?age='.$vCut['kAge'].'&genotype='.$vCut['cGenotype'].'&animal='.$vCut['cAnimal'].'&cutidentifier='.$vCut['cCutIdentifier'].'"><i class="fa fa-pencil"><button type="button" class="btn btn-default">Show Details</button></i></a></td>';
											echo '</tr>';	
										}						
						
			echo '
								</tbody>
							</table>
						</div>
					</div>';				
				
					$tempAnomalies = array();
					$tempAnomalies = $anomaliesCounter['IQR Outlier Test'];
					uasort($tempAnomalies,'compareArrayValueNegativly');
					echo '&emsp;<b>Properties with most anomalies:</b>';
					echo '&emsp;<ul><font size="2">';
					foreach($tempAnomalies as $key => $value) {
						
						echo '<li>'.$key.': '.$value.' x</li>';
						
					}
					echo '</font></ul>	
				</div>				
			</div>';		
			
		}
		
		if(in_array('IQR Outlier Test 1.5', $statisticalMethod)) {
			
			echo '
			<div class="panel-group" id="accordion">
				<div class="panel panel-default" id="panel_IQR_Outlier_Test_1_5">
					<div class="panel-heading">
						<h4 class="panel-title">
							<a data-toggle="collapse" data-target="#collapse_IQR_Outlier_Test_1_5" href="#collapseOptions3"><b>IQR Outlier Test (±1.5 x IQR)</b> <font size="2">('.count($anomalies['IQR Outlier Test 1.5']).' anomal slices detected)</font></a>
						</h4>
					</div>
					<div id="collapse_IQR_Outlier_Test_1_5" class="panel-collapse collapse">
						<div class="panel-body">
							<table class="table table-striped">
								<tbody>';		
								
								readfile("../html/anomaliesTableHead.html");
								
								$tempAnomalies = array();
								$tempAnomalies = $anomalies['IQR Outlier Test 1.5'];
								uasort($tempAnomalies,'compareArraySizeNegatively');
								
								if($anomaliesCounter['IQR Outlier Test 1.5'] == null){
						
									$anomaliesCounter['IQR Outlier Test 1.5'] = array();
								}	
														
										foreach($tempAnomalies as $key => $value) {
											
											$statement  = $pdoLog->prepare('SELECT * FROM vCut WHERE vCut.kCut = '.$key);
											$statement->execute();
											$vCut    = $statement->fetch();
											
											echo '<tr role="row">';			
												echo '<td>'.$vCut['kAge'].'</td>';
												echo '<td>'.$vCut['cGenotype'].'</td>';
												echo '<td>'.$vCut['cAnimal'].'</td>';
												echo '<td>'.$vCut['cCutIdentifier'].'</td>';				
												echo '<td><ul><font size="2">';
												
												$stringAnomaly = "";
												
												foreach($value as $key2 => $jsonObject) {
													
													$anomaly = json_decode($jsonObject);
													
													$cResultType = $anomaly->cResultType;
													$cIdentifier = $anomaly->cIdentifier;
													$cKey = $anomaly->cKey;	
													$limitResult = $anomaly->limitResult;													
													
													$stringAnomaly .= '<li><b>';
													
													$anomalyKeyIdentifier = '';
													
													if($cResultType != 'global') {
														
														$anomalyKeyIdentifier .= $cResultType;
														$anomalyKeyIdentifier .= ' '.$cIdentifier;
													}
													
													$anomalyKeyIdentifier .= ' '.$cKey;
													
													$stringAnomaly .= $anomalyKeyIdentifier;
													
													
													$stringAnomaly .= '</b> '.$limitResult;
													$stringAnomaly .= '</li>';	

													if($anomaliesCounter['IQR Outlier Test 1.5'][$anomalyKeyIdentifier] == null){
														
														$anomaliesCounter['IQR Outlier Test 1.5'][$anomalyKeyIdentifier] = 1;
												
													}
													else
													{
														$anomaliesCounter['IQR Outlier Test 1.5'][$anomalyKeyIdentifier] = $anomaliesCounter['IQR Outlier Test 1.5'][$anomalyKeyIdentifier] + 1;
													}												
													
												}
												
												echo $stringAnomaly;
												
												echo'</font></ul></td>';
												echo '<td><a target="_blank" href="details.php?age='.$vCut['kAge'].'&genotype='.$vCut['cGenotype'].'&animal='.$vCut['cAnimal'].'&cutidentifier='.$vCut['cCutIdentifier'].'"><i class="fa fa-pencil"><button type="button" class="btn btn-default">Show Details</button></i></a></td>';
											echo '</tr>';	
										}						
						
			echo '
								</tbody>
							</table>
						</div>
					</div>';				
				
					$tempAnomalies = array();
					$tempAnomalies = $anomaliesCounter['IQR Outlier Test 1.5'];
					uasort($tempAnomalies,'compareArrayValueNegativly');
					echo '&emsp;<b>Properties with most anomalies:</b>';
					echo '&emsp;<ul><font size="2">';
					foreach($tempAnomalies as $key => $value) {
						
						echo '<li>'.$key.': '.$value.' x</li>';
						
					}
					echo '</font></ul>	
				</div>				
			</div>';	 
		}
		
		#endregion print Output
		
	echo '</div>';
	
	} else {

		echo '<br/><br/><div class="alert alert-danger">
				<strong>At least one animal, one layer, one statistical method and one property is necessary</strong>
			</div><br/><br/>
			<center><a href="anomalies.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Specify Filters</button></a></center>';
			
			readfile("../html/footer.html");
	
	}
?>		
	</body>
</html>