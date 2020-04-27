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

function generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed)
{		
	        require_once('db.php');
		    $pdoLog     = getPDO(1);
	
	        $key = $vCut['cKey'];
			$key_underscore = str_replace(" ", "_", $key);
			
			$unit = $vCut['cUnit'];
			
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
			
			if($cResultType != 'global')
			{
				$key= "$cResultType $cIdentifier $key";				
			}
			$key_underscore = str_replace(" ", "_", $key);
			
			$valuesWT = array();
			$valuesKO = array();
						
			foreach($vCutWTList as $keyWT => $vCutWT) {
				
				array_push($valuesWT, castToFloat($vCutWT['cValue']));
			}
			
			foreach($vCutKOList as $keyKO => $vCutKO) {
				
				array_push($valuesKO, castToFloat($vCutKO['cValue']));
			}
						
			$anzahl_wt = sizeof($valuesWT);
			$anzahl_ko = sizeof($valuesKO);
			$delta_anzahl = number_format((((sizeof($valuesKO) / sizeof($valuesWT)) - 1) * 100), 2, '.', '');			
			$delta_anzahl_normalized = normalize($delta_anzahl);			
			
			$mittelwert_wt = number_format(calculate_average($valuesWT), 2 , '.', '');
			$mittelwert_ko = number_format(calculate_average($valuesKO), 2 , '.', '');
			$delta_mittelwert = number_format((((calculate_average($valuesKO) / calculate_average($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_mittelwert_normalized = normalize($delta_mittelwert);
			
			$varianz_wt = number_format(calculate_varianz($valuesWT), 2 , '.', '');
			$varianz_ko = number_format(calculate_varianz($valuesKO), 2 , '.', '');
			$delta_varianz = number_format((((calculate_varianz($valuesKO) / calculate_varianz($valuesWT)) - 1) * 100), 2, '.', '');
			$delta_varianz_normalized = normalize($delta_varianz);			
			
			$standardabweichung_wt = number_format(calculate_standardabweichung($valuesWT), 2 , '.', '');
			$standardabweichung_ko = number_format(calculate_standardabweichung($valuesKO), 2 , '.', '');
			$delta_standardabweichung = number_format((((calculate_standardabweichung($valuesKO) / calculate_standardabweichung($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_standardabweichung_normalized = normalize($delta_standardabweichung);	
			
			$standardfehlerMittelwert_wt = number_format(calculate_standardfehlerMittelwert($valuesWT), 2 , '.', '');
			$standardfehlerMittelwert_ko = number_format(calculate_standardfehlerMittelwert($valuesKO), 2 , '.', '');
			$delta_standardfehlerMittelwert = number_format((((calculate_standardfehlerMittelwert($valuesKO)  / calculate_standardfehlerMittelwert($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_standardfehlerMittelwert_normalized = normalize($delta_standardfehlerMittelwert);	
			
			$standardfehlerMedian_wt = number_format(calculate_standardfehlerMedian($valuesWT), 2 , '.', '');
			$standardfehlerMedian_ko = number_format(calculate_standardfehlerMedian($valuesKO), 2 , '.', '');
			$delta_standardfehlerMedian = number_format((((calculate_standardfehlerMedian($valuesKO)  / calculate_standardfehlerMedian($valuesWT)) - 1) * 100), 2, '.', '');
			$delta_standardfehlerMedian_normalized = normalize($delta_standardfehlerMedian);				
			
			$varianzkoeffizienz_wt = number_format(calculate_varianzkoeffizient($valuesWT), 2 , '.', '');
			$varianzkoeffizienz_ko = number_format(calculate_varianzkoeffizient($valuesKO), 2 , '.', '');
			$delta_varianzkoeffizienz = number_format((((calculate_varianzkoeffizient($valuesKO) / calculate_varianzkoeffizient($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_varianzkoeffizienz_normalized = normalize($delta_varianzkoeffizienz);			
			
			$quartildispersionskoeffizient_wt = number_format(calculate_quartildispersionskoeffizient($valuesWT), 2 , '.', '');
			$quartildispersionskoeffizient_ko = number_format(calculate_quartildispersionskoeffizient($valuesKO), 2 , '.', '');
			$delta_quartildispersionskoeffizient = number_format((((calculate_quartildispersionskoeffizient($valuesKO) / calculate_quartildispersionskoeffizient($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_quartildispersionskoeffizient_normalized = normalize($delta_quartildispersionskoeffizient);			
			
			$minValueWt = -1;
			$minValueKo = -1;
			
			try 
			{				
				$minValueWt = min($valuesWT);
				
			} catch (Exception $e) {}
			try 
			{				
				$minValueKo = min($valuesKO);
				
			} catch (Exception $e) {}
			
			
			$minimum_wt = number_format($minValueWt, 2 , '.', '');
			$minimum_ko = number_format($minValueKo, 2 , '.', '');
			$delta_minimum = number_format(((($minValueKo / $minValueWt) - 1) * 100), 2, '.', '');	
			$delta_minimum_normalized = normalize($delta_minimum);	
			
			$maxValueWt = 1;
			$maxValueKo = 1;
			
			try 
			{				
				$maxValueWt = max($valuesWT);
				
			} catch (Exception $e) {}
			try 
			{				
				$maxValueKo = max($valuesKO);
				
			} catch (Exception $e) {}
			
			$maximum_wt = number_format($maxValueWt, 2 , '.', '');
			$maximum_ko = number_format($maxValueKo, 2 , '.', '');
			$delta_maximum = number_format(((($maxValueKo / $maxValueWt) - 1) * 100), 2, '.', '');	
			$delta_maximum_normalized = normalize($delta_maximum);	
			
			$median_wt = number_format(calculate_median($valuesWT), 2 , '.', '');
			$median_ko = number_format(calculate_median($valuesKO), 2 , '.', '');
			$delta_median = number_format((((calculate_median($valuesKO) / calculate_median($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_median_normalized = normalize($delta_median);	
			
			$spannweite_wt = abs($maxValueWt - $minValueWt);
			$spannweite_ko = abs($maxValueKo - $minValueKo);
			$delta_spannweite = number_format((((abs($maximum_ko - $minimum_ko) / abs($maximum_wt - $minimum_wt)) - 1) * 100), 2, '.', '');	
			$delta_spannweite_normalized = normalize($delta_spannweite);	
			
			$mittlereAbweichung_wt = number_format(calculate_meanAbsoluteDeviation($valuesWT), 2 , '.', '');
			$mittlereAbweichung_ko = number_format(calculate_meanAbsoluteDeviation($valuesKO), 2 , '.', '');
			$delta_mittlereAbweichung = number_format((((calculate_meanAbsoluteDeviation($valuesKO) / calculate_meanAbsoluteDeviation($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_mittlereAbweichung_normalized = normalize($delta_mittlereAbweichung);	
			
			$medianAbweichung_wt = number_format(calculate_medianAbsoluteDeviation($valuesWT), 2 , '.', '');
			$medianAbweichung_ko = number_format(calculate_medianAbsoluteDeviation($valuesKO), 2 , '.', '');
			$delta_medianAbweichung = number_format((((calculate_medianAbsoluteDeviation($valuesKO) / calculate_medianAbsoluteDeviation($valuesWT)) - 1) * 100), 2, '.', '');	
			$delta_medianAbweichung_normalized = normalize($delta_medianAbweichung);

			$erstesQuartil_wt = quartile($valuesWT, 0.25);
			$drittesQuartil_wt = quartile($valuesWT, 0.75);	

			$erstesQuartil_ko = quartile($valuesKO, 0.25);
			$drittesQuartil_ko = quartile($valuesKO, 0.75);		

//		interval: '.number_format((($mittelwert_wt + $mittelwert_ko)/2),2,'.','').'	
	
			//$interval = $spannweite_wt;
			//if($spannweite_ko > $interval) $interval = $spannweite_ko;
			
            // $interval = abs($drittesQuartil_wt);
			// if(abs($drittesQuartil_ko) > $interval) $interval = abs($drittesQuartil_ko);
			// $interval = (($interval / 3) * 4) / 2;
			
			$interval_max = $maxValueWt;
			if($maxValueKo > $interval_max) $interval_max = $maxValueKo;
			
			$interval_min = $minValueWt;
			if($minValueKo < $interval_min) $interval_min = $minValueKo;
			
			if($interval_max < $interval_min){
				
				$tmp = $interval_max;
				$interval_max = $interval_min;
				$interval_min = $tmp;
			}
			
			$interval = abs($interval_max - $interval_min);
			
			$axisYMax = $interval_max + $interval;
			$axisYMin = $interval_min - $interval;
			
			echo '
			<div class="panel panel-default">
  <div class="panel-body">
  
  <table>
  <tr>
    <td style="width:25%">
		<h4><b>'.$key.'</b></h4>
	</td>
	<td style="width:25%" rowspan="2">
		<div style="width:250;height:250" id="chartContainer_'.$key_underscore.'"></div>
	</td>
	<td style="width:50%" rowspan="2">
		<div style="width:570;height:260">
		<canvas id="WT_VS_KO_'.$key_underscore.'"></canvas>
		</div>
	</td>
	
<script>


var options_'.$key_underscore.' = {
	animationEnabled: true,
	width: 250,
	height: 250,
	dataPointMaxWidth: 10,
	title: {
		text: ""
	},
	axisY: {
		title: "",
		suffix: " '.$unit.'",
		interval: '.number_format(($interval),2,'.','').',
		minimum: '.number_format(($axisYMin),2,'.','').',
		maximum: '.number_format(($axisYMax),2,'.','').',
	},
	axisX:{     
     margin: 10
    },
	data: [{
		type: "boxAndWhisker",
		upperBoxColor: "#68D46F",
		lowerBoxColor: "#8062EF",
		color: "black",
		yValueFormatString: "#.#0 '.$unit.'",
		dataPoints: [
			{ label: "WT", y: ['.number_format($minimum_wt,2,'.','').', '.number_format($erstesQuartil_wt,2,'.','').', '.number_format($drittesQuartil_wt,2,'.','').', '.number_format($maximum_wt,2,'.','').', '.number_format($median_wt,2,'.','').'] },
			{ label: "KO", y: ['.number_format($minimum_ko,2,'.','').', '.number_format($erstesQuartil_ko,2,'.','').', '.number_format($drittesQuartil_ko,2,'.','').', '.number_format($maximum_ko,2,'.','').', '.number_format($median_ko,2,'.','').'] }
		]
	}]
};

$("#chartContainer_'.$key_underscore.'").CanvasJSChart(options_'.$key_underscore.');





var ctx_WT_VS_KO_'.$key_underscore.' = document.getElementById(\'WT_VS_KO_'.$key_underscore.'\');

var myChart = new Chart(ctx_WT_VS_KO_'.$key_underscore.', {
    type: \'bar\',
    data: {
        labels: ['.join(", ", $statisticalMethodStringed).'],
        datasets: [{
            label: \'Δ WT vs. KO [%] (normalized: |200|)\',
            data: ['.generateData($statisticalMethod, $delta_mittelwert_normalized, $delta_varianz_normalized, $delta_standardabweichung_normalized, $delta_standardfehlerMittelwert_normalized, $delta_standardfehlerMedian_normalized, $delta_varianzkoeffizienz_normalized, $delta_quartildispersionskoeffizient_normalized, $delta_minimum_normalized, $delta_maximum_normalized, $delta_median_normalized, $delta_spannweite_normalized, $delta_mittlereAbweichung_normalized, $delta_medianAbweichung_normalized).'],
            backgroundColor: [
                \'rgba(255, 99, 132, 0.2)\',
                \'rgba(54, 162, 235, 0.2)\'
            ],
            borderColor: [
                \'rgba(255, 99, 132, 1)\',
                \'rgba(54, 162, 235, 1)\'
            ],
            borderWidth: 1
        }]
    },
    options: {
		// Boolean - whether or not the chart should be responsive and resize when the browser does.
		responsive: true,

		// Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
		maintainAspectRatio: false,
		
		legend: {
			display: true,
			labels: {
				boxWidth: 0,
			}
		},
        scales: {
			xAxes: [{
				barThickness : 30,
				categorySpacing: 100
			}],	
            yAxes: [{
                ticks: {
                    beginAtZero: true,
					//min: -100,
					//max: 100,
					//stepSize: 50
                }
            }]
        }
    }
});
</script>

  </tr>
  <tr>
    <td>
		<p>
		  <button class="btn btn-info" type="button" data-toggle="collapse" data-target="#collapse_'.$key_underscore.'" aria-expanded="false" aria-controls="collapse_'.$key_underscore.'">
			Show Details
		  </button>
		</p>
	</td>
  </tr>
</table>					
	<div class="collapse" id="collapse_'.$key_underscore.'">
	  <div class="card card-body">
		<br/><br/>
		<table style="width:100%">
			<tr>
				<td style="width:60%">
					
					<table class="table table-striped">
						<tbody>
							<tr>
								<th style="width:40%"></th>
								<th style="width:20%" scope="row">WT</th>
								<th style="width:20%" scope="row">KO</th>
								<th style="width:20%" scope="row">Δ [%]</th>
							</tr>							
							<tr>
								<td>Anzahl Datensätze</td>
								<td>'.$anzahl_wt.'</td>
								<td>'.$anzahl_ko.'</td>
								<td><b>'.$delta_anzahl.'</b></td>
							</tr>
							';							
							if(in_array('Mittelwert', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Mittelwert</td>
								<td>'.$mittelwert_wt.'</td>
								<td>'.$mittelwert_ko.'</td>
								<td><b>'.$delta_mittelwert.'</b></td>
							</tr>
							';
							}
							if(in_array('Varianz', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Varianz</td>
								<td>'.$varianz_wt.'</td>
								<td>'.$varianz_ko.'</td>
								<td><b>'.$delta_varianz.'</b></td>
							</tr>
							';
							}
							if(in_array('Standard Abweichung', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Standard Abweichung</td>
								<td>'.$standardabweichung_wt.'</td>
								<td>'.$standardabweichung_ko.'</td>
								<td><b>'.$delta_standardabweichung.'</b></td>
							</tr>
							';
							}
							if(in_array('Standard Fehler Mittelwert', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Standard Fehler Mittelwert</td>
								<td>'.$standardfehlerMittelwert_wt.'</td>
								<td>'.$standardfehlerMittelwert_ko.'</td>
								<td><b>'.$delta_standardfehlerMittelwert.'</b></td>
							</tr>
							';
							}
							if(in_array('Standard Fehler Median', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Standard Fehler Median</td>
								<td>'.$standardfehlerMedian_wt.'</td>
								<td>'.$standardfehlerMedian_ko.'</td>
								<td><b>'.$delta_standardfehlerMedian.'</b></td>
							</tr>
							';
							}
							if(in_array('Varianz Koeffizient', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Varianz Koeffizient</td>
								<td>'.$varianzkoeffizienz_wt.'</td>
								<td>'.$varianzkoeffizienz_ko.'</td>
								<td><b>'.$delta_varianzkoeffizienz.'</b></td>
							</tr>
							';
							}
							if(in_array('Quartil Dispersionskoeffizient', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Quartil Dispersionskoeffizient </td>
								<td>'.$quartildispersionskoeffizient_wt.'</td>
								<td>'.$quartildispersionskoeffizient_ko.'</td>
								<td><b>'.$delta_quartildispersionskoeffizient.'</b></td>
							</tr>
							';
							}
							if(in_array('Minimum', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Minimum</td>
								<td>'.$minimum_wt.'</td>
								<td>'.$minimum_ko.'</td>
								<td><b>'.$delta_minimum.'</b></td>
							</tr>
							';
							}
							if(in_array('Maximum', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Maximum</td>
								<td>'.$maximum_wt.'</td>
								<td>'.$maximum_ko.'</td>
								<td><b>'.$delta_maximum.'</b></td>
							</tr>
							';
							}
							if(in_array('Median', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Median</td>
								<td>'.$median_wt.'</td>
								<td>'.$median_ko.'</td>
								<td><b>'.$delta_median.'</b></td>
							</tr>
							';
							}
							if(in_array('Spannweite', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Spannweite</td>
								<td>'.$spannweite_wt.'</td>
								<td>'.$spannweite_ko.'</td>
								<td><b>'.$delta_spannweite.'</b></td>
							</tr>
							';
							}
							if(in_array('Mittlere Abweichung', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Mittlere Abweichung</td>
								<td>'.$mittlereAbweichung_wt.'</td>
								<td>'.$mittlereAbweichung_ko.'</td>
								<td><b>'.$delta_mittlereAbweichung.'</b></td>
							</tr>
							';
							}
							if(in_array('Median Abweichung', $statisticalMethod)) 
							{
							echo '
							<tr>
								<td>Median Abweichung</td>
								<td>'.$medianAbweichung_wt.'</td>
								<td>'.$medianAbweichung_ko.'</td>
								<td><b>'.$delta_medianAbweichung.'</b></td>
							</tr>
							';
							}
							echo '
						</tbody>
					</table>	
					
				</td>
				<td style="width:40%">
					<!--<div style="width:500">-->
						<canvas id="RADAR_'.$key_underscore.'"></canvas>
					<!--</div>-->
				</td>
			</tr>
		</table>
				
		<script>
var ctx_RADAR_'.$key_underscore.' = document.getElementById(\'RADAR_'.$key_underscore.'\');

var myChart = new Chart(ctx_RADAR_'.$key_underscore.', {
    type: \'radar\',
    data: {
        labels: ['.join(", ", $statisticalMethodStringed).'],
		datasets: [{
			label: "Δ [%]",
			backgroundColor: "rgba(0, 230, 64,0.2)",
			data: ['.generateDataRadarDiagram($statisticalMethod, $delta_mittelwert, $delta_varianz, $delta_standardabweichung, $delta_standardfehlerMittelwert, $delta_standardfehlerMedian, $delta_varianzkoeffizienz, $delta_quartildispersionskoeffizient, $delta_minimum, $delta_maximum, $delta_median, $delta_spannweite, $delta_mittlereAbweichung, $delta_medianAbweichung).']
		  }]
    },
    options: {
		// Boolean - whether or not the chart should be responsive and resize when the browser does.
		responsive: true,

		// Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
		maintainAspectRatio: false,
		
		 tooltips: {
                 mode: \'label\',
                callbacks: {
                     label: function(tooltipitem, data) { 
					
					 return data.datasets[tooltipitem.datasetindex].label + \': \' + data.datasets[tooltipitem.datasetindex].data[tooltipitem.index];

					 }
                 }
             }
		
    }
});
</script>		
		
	  </div>
	</div>
  </div>
</div>';
		
	
}

	$ageFrom           = $_POST['ageFrom'];
	$ageTo             = $_POST['ageTo'];
	$animals           = $_POST['animals'];
	$layer             = $_POST['layer'];
	$statisticalMethod = $_POST['statisticalMethod'];
	
    readfile("../html/head.html");	
	
	if
	(
		(is_null($ageFrom) == false && isset($ageFrom) && trim($ageFrom) !== '') &&
		(is_null($ageTo) == false && isset($ageTo) && trim($ageTo) !== '') &&
		(is_null($animals) == false && isset($animals)) &&
		(is_null($layer) == false && isset($layer)) &&
		(is_null($statisticalMethod) == false && isset($statisticalMethod))
	)	
	{		
	
	$statisticalMethodStringed = $statisticalMethod;
	
	foreach($statisticalMethod as $key=>$value){
	  $statisticalMethod[$key]=str_replace("'",'',$value);
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
		echo '<a href="analytics.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Specify Filters</button></a>';
		echo '<h1>Analytics</h1><br/>';
		echo '<h3>Specified Filters:</h3>';
		echo '<h5>&emsp;Age from '.$ageFrom.' to '.$ageTo.'</h5>';
		echo '<h5>&emsp;Animals: '.str_replace("'", '', join(', ', $animals)).'</h5>';
		echo '<h5>&emsp;Layer: '.str_replace("'", '', join(', ', $layer)).'</h5>';
		echo '<br/><br/>';
		
		require_once('db.php');
		$pdoLog     = getPDO(1);		

		$cResultType = 'global';
		$cIdentifier = '';
		$pId = $cResultType.'_'.$cIdentifier;
		
		echo '
		<div class="panel-group" id="accordion">
			<div class="panel panel-default" id="panel_'.$pId.'">
				<div class="panel-heading">
					<h4 class="panel-title">
						<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
					</h4>
				</div>
				<div id="collapse_'.$pId.'" class="panel-collapse collapse">
					<div class="panel-body">';		
													
		
						$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' ORDER BY vResult.cKey");
						$statement->execute();
						$vCutKeysGlobal    = $statement->fetchAll();
		
						foreach($vCutKeysGlobal as $key => $vCut) {
							
							generateDetails($query, $vCut, $cResultType, $pId, $statisticalMethod, $statisticalMethodStringed);			
						}
		
		echo '
					</div>
				</div>
			</div>
		</div>';
		
		$cResultType = 'fissure';
		$cIdentifier = '';
		$pId = $cResultType.'_'.$cIdentifier;
		
		echo '
		<div class="panel-group" id="accordion">
			<div class="panel panel-default" id="panel_'.$pId.'">
				<div class="panel-heading">
					<h4 class="panel-title">
						<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
					</h4>
				</div>
				<div id="collapse_'.$pId.'" class="panel-collapse collapse">
					<div class="panel-body">';							
					
					
						$cIdentifier = 'Ø';
						$pId = $cResultType.'_'.$cIdentifier;
						
						echo '
						<div class="panel-group" id="accordion">
							<div class="panel panel-default" id="panel_'.$pId.'">
								<div class="panel-heading">
									<h4 class="panel-title">
										<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
									</h4>
								</div>
								<div id="collapse_'.$pId.'" class="panel-collapse collapse">
									<div class="panel-body">';		
																	
						
										$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
										$statement->execute();
										$vCutKeysGlobal    = $statement->fetchAll();

										foreach($vCutKeysGlobal as $key => $vCut) {
											
											generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
										}
						
						echo '
									</div>
								</div>
							</div>
						</div>';	

						
						$cIdentifier = 'Σ';
						$pId = $cResultType.'_'.$cIdentifier;
						
						echo '
						<div class="panel-group" id="accordion">
							<div class="panel panel-default" id="panel_'.$pId.'">
								<div class="panel-heading">
									<h4 class="panel-title">
										<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
									</h4>
								</div>
								<div id="collapse_'.$pId.'" class="panel-collapse collapse">
									<div class="panel-body">';		
																	
						
										$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
										$statement->execute();
										$vCutKeysGlobal    = $statement->fetchAll();

										foreach($vCutKeysGlobal as $key => $vCut) {
											
											generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
										}
						
						echo '
									</div>
								</div>
							</div>
						</div>';


						for($i = 1; $i <= 10; $i++){
			
							$cIdentifier = $i;
							$pId = $cResultType.'_'.$cIdentifier;
							
							echo '
							<div class="panel-group" id="accordion">
								<div class="panel panel-default" id="panel_'.$pId.'">
									<div class="panel-heading">
										<h4 class="panel-title">
											<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
										</h4>
									</div>
									<div id="collapse_'.$pId.'" class="panel-collapse collapse">
										<div class="panel-body">';		
																		
							
											$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
											$statement->execute();
											$vCutKeysGlobal    = $statement->fetchAll();

											foreach($vCutKeysGlobal as $key => $vCut) {
												
												generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
											}
							
							echo '
										</div>
									</div>
								</div>
							</div>';			
						}						
		
		echo '
					</div>
				</div>
			</div>
		</div>';	
		
		$cResultType = 'lobule';
		$cIdentifier = '';
		$pId = $cResultType.'_'.$cIdentifier;
		
		echo '
		<div class="panel-group" id="accordion">
			<div class="panel panel-default" id="panel_'.$pId.'">
				<div class="panel-heading">
					<h4 class="panel-title">
						<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
					</h4>
				</div>
				<div id="collapse_'.$pId.'" class="panel-collapse collapse">
					<div class="panel-body">';							
											
						$cIdentifier = 'Ø';
						$pId = $cResultType.'_'.$cIdentifier;
						
						echo '
						<div class="panel-group" id="accordion">
							<div class="panel panel-default" id="panel_'.$pId.'">
								<div class="panel-heading">
									<h4 class="panel-title">
										<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
									</h4>
								</div>
								<div id="collapse_'.$pId.'" class="panel-collapse collapse">
									<div class="panel-body">';		
																	
						
										$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
										$statement->execute();
										$vCutKeysGlobal    = $statement->fetchAll();

										foreach($vCutKeysGlobal as $key => $vCut) {
											
											generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
										}
						
						echo '
									</div>
								</div>
							</div>
						</div>';	

						
						$cIdentifier = 'Σ';
						$pId = $cResultType.'_'.$cIdentifier;
						
						echo '
						<div class="panel-group" id="accordion">
							<div class="panel panel-default" id="panel_'.$pId.'">
								<div class="panel-heading">
									<h4 class="panel-title">
										<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
									</h4>
								</div>
								<div id="collapse_'.$pId.'" class="panel-collapse collapse">
									<div class="panel-body">';		
																	
						
										$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
										$statement->execute();
										$vCutKeysGlobal    = $statement->fetchAll();

										foreach($vCutKeysGlobal as $key => $vCut) {
											
											generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
										}
						
						echo '
									</div>
								</div>
							</div>
						</div>';


						for($i = 1; $i <= 10; $i++){
			
							$cIdentifier = $i;
							$pId = $cResultType.'_'.$cIdentifier;
							
							echo '
							<div class="panel-group" id="accordion">
								<div class="panel panel-default" id="panel_'.$pId.'">
									<div class="panel-heading">
										<h4 class="panel-title">
											<a data-toggle="collapse" data-target="#collapse_'.$pId.'" href="#collapseOptions"><b>'.$cResultType.' '.$cIdentifier.'</b></a>
										</h4>
									</div>
									<div id="collapse_'.$pId.'" class="panel-collapse collapse">
										<div class="panel-body">';		
																		
							
											$statement  = $pdoLog->prepare('SELECT DISTINCT vResult.cKey, vResult.cUnit '.$query." AND vResult.cResultType = '".$cResultType."' AND vResult.cIdentifier = '".$cIdentifier."' ORDER BY vResult.cKey");
											$statement->execute();
											$vCutKeysGlobal    = $statement->fetchAll();

											foreach($vCutKeysGlobal as $key => $vCut) {
												
												generateDetails($query, $vCut, $cResultType, $cIdentifier, $statisticalMethod, $statisticalMethodStringed);					
											}
							
							echo '
										</div>
									</div>
								</div>
							</div>';			
						}						
		
		echo '
					</div>
				</div>
			</div>
		</div>';	

		
	echo '</div>';
	
	} else {

		echo '<br/><br/><div class="alert alert-danger">
				<strong>At least one animal, one layer and one statistical method is necessary</strong>
			</div><br/><br/>
			<center><a href="analytics.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Specify Filters</button></a></center>';
			
			readfile("../html/footer.html");
	
	}
?>		
	</body>
</html>