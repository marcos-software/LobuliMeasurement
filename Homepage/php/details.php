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

	$age           = $_GET['age'];
	$genotype      = $_GET['genotype'];
	$animal        = $_GET['animal'];
	$cutidentifier = $_GET['cutidentifier'];
    
	
	if
	(
		(is_null($age) == false && isset($age) && trim($age) !== '') &&
		(is_null($genotype) == false && isset($genotype) && trim($genotype) !== '') &&
		(is_null($animal) == false && isset($animal) && trim($animal) !== '') &&
		(is_null($cutidentifier) == false && isset($cutidentifier) && trim($cutidentifier) !== '')
	)	
	{		
	    echo '<html lang="en">';
		readfile("../html/head.html");	
		require_once('db.php');
			
		$pdoLog                 = getPDO(1);	
		$pdoProcedureF          = getPDO(1);		
		$pdoProcedureL          = getPDO(1);	
				
		$statement              = $pdoLog->prepare("SELECT * FROM vCut WHERE kAge = :kAge AND cGenotype = :cGenotype AND cAnimal = :cAnimal AND cCutIdentifier = :cCutIdentifier");
		$statement->execute(array('kAge' => $age, 'cGenotype' => $genotype, 'cAnimal' => $animal, 'cCutIdentifier' => $cutidentifier));
		$vCut                   = $statement->fetch();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vCoordinate WHERE kCut = :kCut");
		$statement->execute(array('kCut' => $vCut['kCut']));
		$vCoordinates           = $statement->fetchAll();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vImage WHERE kCut = :kCut AND cType = :cType");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cType' => 'analyzed image'));
		$vImageAnalyzed         = $statement->fetch();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vImage WHERE kCut = :kCut AND cType = :cType");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cType' => 'marked image'));
		$vImageMarked           = $statement->fetch();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vImage WHERE kCut = :kCut AND cType = :cType");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cType' => 'original image'));
		$vImageOriginal         = $statement->fetch();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vImage WHERE kCut = :kCut AND cType = :cType");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cType' => 'white matter area image'));
		$vImageWhiteMatter      = $statement->fetch();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vOption WHERE kCut = :kCut");
		$statement->execute(array('kCut' => $vCut['kCut']));
		$vOptions               = $statement->fetchAll();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vResult WHERE kCut = :kCut AND cResultType = :cResultType");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cResultType' => 'global'));
		$vResultsGlobal         = $statement->fetchAll();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vResult WHERE kCut = :kCut AND cResultType = :cResultType");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cResultType' => 'white matter'));
		$vResultsWhiteMatter    = $statement->fetchAll();
		
		$statement              = $pdoLog->prepare("SELECT * FROM vResult WHERE kCut = :kCut AND cResultType != :cResultType1 AND cResultType != :cResultType2 AND cIdentifier IS NULL");
		$statement->execute(array('kCut' => $vCut['kCut'], 'cResultType1' => 'global', 'cResultType2' => 'white matter'));
		$vResultsCount          = $statement->fetchAll();			
			
		$statement              = $pdoProcedureF->prepare("CALL spCreateResultPivotStmt(:in_kCut, :in_cResultType);");
		$statement->execute(array('in_kCut' => $vCut['kCut'], 'in_cResultType' => 'fissure'));
		$resultFisurPivotStmt   = $statement->fetch();		
				
		$statement              = $pdoLog->prepare((string)($resultFisurPivotStmt[0]));
		$statement->execute();
		$vResultsFisur          = $statement->fetchAll();			
		
		$statement              = $pdoProcedureL->prepare("CALL spCreateResultPivotStmt(:in_kCut, :in_cResultType);");
		$statement->execute(array('in_kCut' => $vCut['kCut'], 'in_cResultType' => 'lobule'));
		$resultLobulusPivotStmt = $statement->fetch();
				
		$statement              = $pdoLog->prepare((string)($resultLobulusPivotStmt[0]));
		$statement->execute();
		$vResultsLobulus        = $statement->fetchAll();
		
		$vCut['cNote'] = trim($vCut['cNote'] , "<br />");
		
		$excludeButtonClass = "btn btn-danger";
		$excludeText = 'Exclude from analysis';
		
		if($vCut['bExcludeFromAnalysis'] == '1')
		{
			$excludeButtonClass = "btn btn-success";
			$excludeText = 'Include in analysis';
		}
		
	
	// readfile("../html/details.html");	
	
	$lastpage = 'overview.php';		
	$lastpageName = 'Overview';
		
	try 
	{		
		$lastpage = $_SERVER['HTTP_REFERER'];
		
		if(is_null($lastpage) || isset($lastpage) == false || trim($lastpage) === '')
		{
			$lastpage = 'overview.php';		
		}
		
		$lastpage = strtok($lastpage, '?');
		
		$lastpageName = ucfirst(str_replace ('_' , ' ' , str_replace ('.php' , '' , basename($lastpage)))); 
		
		if($lastpageName == 'Details')
		{
			$lastpageName = 'Menu';
		}
	} 
	catch (Exception $e) 
	{
		
		$lastpageName = 'Overview';
	}

	
	echo '<body>
	<div id="masthead">  
	  <div class="container">
	    <br>
		<a href="'.$lastpage.'" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show '.$lastpageName.'</button></a>
		  <div class="row">
			<div class="col-md-6">
			<br />
			  <h1>Slice
				<p class="lead"># '.$vCut['kCut'].'</p>
			  </h1>
				<table class="table table-striped">
					<tbody>
						<tr>
						  <th scope="row">Age</th>
						  <td>'.$vCut['kAge'].'</td>
						</tr>
						<tr>
						  <th scope="row">Genotype</th>
						  <td>'.($vCut['cGenotype'] == 'KO' ? '<span class="label label-info">'.$vCut['cGenotype'].'</span>' : '<span class="label label-success">'.$vCut['cGenotype'].'</span>')   .'</td>
						</tr>
						<tr>
						  <th scope="row">Animal</th>
						  <td>'.$vCut['cAnimal'].'</td>
						</tr>
						<tr>
						  <th scope="row">Cut Identifier</th>
						  <td>'.$vCut['cCutIdentifier'].'</td>
						</tr>
						<tr>
						  <th scope="row">Method</th>
						  <td>'.$vCut['cMethod'].'</td>
						</tr>
						<tr>
						  <th scope="row">Date Measurement</th>
						  <td>'.$vCut['dDateMeasurement'].'</td>
						</tr>
						<tr>
						  <th scope="row">Date Staining</th>
						  <td>'.$vCut['dDateStaining'].'</td>
						</tr>
						<tr>
						  <th scope="row">Zoom Factor</th>
						  <td>'.$vCut['fZoomFactor'].'</td>
						</tr>
						<tr>
						  <th scope="row">Layer</th>
						  <td>'.$vCut['cLayer'].'</td>
						</tr>

					  </tbody>
				</table>
				
			<div class="panel panel-default">		

				<div class="panel-body">
					<b>Note</b>
				  </div>
				  <div class="panel-footer">'.$vCut['cNote'].'</div>
				</div>						
			
				<form class="form-inline" action="addNote.php" method="post">
					<input type="hidden" name="cutid" value="'.$vCut['kCut'].'">
					<input type="hidden" name="age" value="'.$age.'">
					<input type="hidden" name="genotype" value="'.$genotype.'">
					<input type="hidden" name="animal" value="'.$animal.'">
					<input type="hidden" name="cutidentifier" value="'.$cutidentifier.'">
					<input type="text"   name="note" class="form-control" placeholder="new note" style="width: 80%;">
				    <button type="submit" class="btn" style="width: 19%;">Add Note</button>
				</form>	

				<form class="form-inline" action="exclude.php" method="post">
					<input type="hidden" name="cutid" value="'.$vCut['kCut'].'">
					<input type="hidden" name="age" value="'.$age.'">
					<input type="hidden" name="genotype" value="'.$genotype.'">
					<input type="hidden" name="animal" value="'.$animal.'">
					<input type="hidden" name="cutidentifier" value="'.$cutidentifier.'">
				    <button type="submit" class="'.$excludeButtonClass.'" style="width: 100%;">'.$excludeText.'</button>
				</form>				

			</div>			
			
			<div class="col-md-6">
				<div class="well well-lg"> 
				  <div class="row">
					<div class="col-sm-12">
						<div class="thumbnail">
							<a href="#" class="pop">
								<img src="data:image/png;base64,'.base64_encode($vImageAnalyzed['cImage']).'" alt="analyzed image">							 
							<div class="caption">analyzed image</div>
							</a> 
						</div>
					</div>					
				  </div>
				  <div class="row">
					<div class="col-sm-6">
						<div class="thumbnail">
							<a href="#" class="pop">
								<img src="data:image/png;base64,'.base64_encode($vImageMarked['cImage']).'" alt="marked image">
								<div class="caption">marked image</div>
							</a>
						</div>					
					</div>
					<div class="col-sm-6">
						<div class="thumbnail">
							<a href="#" class="pop">
								<img src="data:image/png;base64,'.base64_encode($vImageOriginal['cImage']).'" alt="original image">
								<div class="caption">original image</div>
								</a>
						</div>
					</div>
				  </div>
				</div>
			</div>
		  </div> 
	  </div><!--/container-->
	</div><!--/masthead-->
	<!--main-->
	<div class="container">			

		<table class="table table-striped">
			<tbody>';
	
				foreach($vResultsGlobal as $key => $vResult) 
				{
					echo '
						<tr>
							<th scope="row">'.$vResult['cKey'].'</th>
							<td>'.$vResult['cValue'].'&nbsp;'.$vResult['cUnit'].'</td>
						</tr>';	
				}
				
				foreach($vResultsCount as $key => $vResult) 
				{
					echo '
						<tr>
							<th scope="row">'.$vResult['cKey'].'</th>
							<td>'.$vResult['cValue'].'&nbsp;'.$vResult['cUnit'].'</td>
						</tr>';	
				}				
			echo '
			</tbody>
		</table>
		
		<br />
		
		<div class="panel-group" id="accordion">
			<div class="panel panel-default" id="panelWhiteMatter">
				<div class="panel-heading">
					<h4 class="panel-title">
						<a data-toggle="collapse" data-target="#collapseWhiteMatter" href="#collapseWhiteMatter"><b>white matter details</b></a>
					</h4>
				</div>
				<div id="collapseWhiteMatter" class="panel-collapse collapse">
					<div class="panel-body">
						<table class="table table-striped">
							<tbody>';			
					
								foreach($vResultsWhiteMatter as $key => $vResult) 
								{
									echo '
										<tr>
											<th scope="row">'.$vResult['cKey'].'</th>
											<td>'.$vResult['cValue'].'&nbsp;'.$vResult['cUnit'].'</td>
										</tr>';	
								}		
											
							echo '
							</tbody>
						</table>
						
						<br />
						
						<div class="col-sm-6">
						<div class="thumbnail">
							<a href="#" class="pop">
								<img src="data:image/png;base64,'.base64_encode($vImageWhiteMatter['cImage']).'" alt="white matter image">
								<div class="caption">white matter image</div>
								</a>
						</div>
					</div>
					</div>
				</div>
			</div>
		</div>	
		
		<br />
		
		<table class="table table-striped">
			<thead>
				<tr>';
		
					foreach(array_keys($vResultsFisur[0]) as $columnName)		
					{
						if(is_string($columnName))
						{
							$name = $columnName;
							if($name == 'identifier')
							{           						
								$name = 'fissure &nbsp; &nbsp; &nbsp;';
							}
							echo "<th scope='col'>$name</th>";
						}
					}
		
		echo '
					<th scope="col"></th>
				</tr>
			</thead> 
			<tbody>';
			     
								
				foreach($vResultsFisur as $key => $row) 
				{					
					$fisureIdentifier = $row['identifier'];
				
					echo '<tr>';						
						
						foreach(array_keys($row) as $columnName)
						{
							if(is_string($columnName))
							{
								echo "<td>$row[$columnName]</td>";					
							}							
						}		
											
					echo '
					
					<td>';
					
					if($fisureIdentifier != 'Ø' && $fisureIdentifier != 'Σ')
					{
						echo '
							
							<form class="form-inline" action="moredetails.php" method="post" style="height: 18px;">
								<input type="hidden" name="cutid" value="'.$vCut['kCut'].'">
								<input type="hidden" name="resulttype" value="Fissure">
								<input type="hidden" name="cIdentifier" value="'.$fisureIdentifier.'">

								<input type="hidden" name="age" value="'.$age.'">
								<input type="hidden" name="genotype" value="'.$genotype.'">
								<input type="hidden" name="animal" value="'.$animal.'">
								<input type="hidden" name="cutidentifier" value="'.$cutidentifier.'">

								<button type="submit" class="btn" style="width: 100%;">Details</button>						
				
								
								
							</form>
							
							';
					}					
					
					echo '</td>					
					</tr>';	
				}
							
		echo '
			</tbody>
		</table>
		
		<table class="table table-striped">
			<thead>
				<tr>';
		
					foreach(array_keys($vResultsLobulus[0]) as $columnName)		
					{
						if(is_string($columnName))
						{
							$name = $columnName;
							if($name == 'identifier')
							{
								$name = 'lobule';
							}
							echo "<th scope='col'>$name</th>";
						}
					}
		
		echo '
					<th scope="col"></th>
				</tr>
			</thead> 
			<tbody>';
			
				foreach($vResultsLobulus as $key => $row) 
				{
					$lobuleIdentifier = $row['identifier'];
					
					echo '<tr>';						
						
						foreach(array_keys($row) as $columnName)
						{
							if(is_string($columnName))
							{
								echo "<td>$row[$columnName]</td>";
							}
							
						}
												
					echo '
					
					<td>';
					
					if($lobuleIdentifier != 'Ø' && $lobuleIdentifier != 'Σ')
					{
						echo '
							
							<form class="form-inline" action="moredetails.php" method="post" style="height: 18px;">
								<input type="hidden" name="cutid" value="'.$vCut['kCut'].'">
								<input type="hidden" name="resulttype" value="Lobule">
								<input type="hidden" name="cIdentifier" value="'.$lobuleIdentifier.'">
								
								<input type="hidden" name="age" value="'.$age.'">
								<input type="hidden" name="genotype" value="'.$genotype.'">
								<input type="hidden" name="animal" value="'.$animal.'">
								<input type="hidden" name="cutidentifier" value="'.$cutidentifier.'">
								
								<button type="submit" class="btn" style="width: 100%;">Details</button>
							</form>
							
							';
					}					
					
					echo '</td>					
					</tr>';	
				}
							
		echo '
			</tbody>
		</table>

		
		<div class="panel-group" id="accordion">
			<div class="panel panel-default" id="panelOptions">
				<div class="panel-heading">
					<h4 class="panel-title">
						<a data-toggle="collapse" data-target="#collapseOptions" href="#collapseOptions"><b>Options</b></a>
					</h4>
				</div>
				<div id="collapseOptions" class="panel-collapse collapse">
					<div class="panel-body">
						<table class="table table-striped">
							<tbody>';
					
								foreach($vOptions as $key => $vOption) 
								{
			
									echo '
										<tr>
											<th scope="row">'.$vOption['cKey'].'</th>
											<td>'.$vOption['cValue'].'&nbsp;'.$vOption['cUnit'].'</td>
										</tr>';	
								}					
							echo '
							</tbody>
						</table>
					</div>
				</div>
			</div>
		</div>
		
		<div class="panel-group" id="accordion">
			<div class="panel panel-default" id="panelCoordinates">
				<div class="panel-heading">
					<h4 class="panel-title">
						<a data-toggle="collapse" data-target="#collapseCoordinates" href="#collapseCoordinates"><b>Coordinates</b></a>
					</h4>
				</div>
				<div id="collapseCoordinates" class="panel-collapse collapse">
					<div class="panel-body">
						<table class="table table-striped">
							<thead>
								<tr>
									<th scope="col">X</th>
									<th scope="col">Y</th>
								</tr>
							</thead>
							<tbody>';
					
								foreach($vCoordinates as $key => $vCoordinate) 
								{
			
									echo '
										<tr>
											<td>'.$vCoordinate['X'].'</td>
											<td>'.$vCoordinate['Y'].'</td>
										</tr>';	
								}				
							echo '
							</tbody>
						</table>
					</div>
				</div>
			</div>
		</div>
		
		
	</div><!--/container-->';


	readfile("../html/footer.html");	
	
	echo '
	<div class="modal fade" id="imagemodal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">              
				<div class="modal-body">
					<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
					<img src="" class="imagepreview" style="width: 100%;" >
				</div>
			</div>
		</div>
	</div>';
	
	echo '</body>';
	}
	else 
	{
		header("Location: https://YOUR_BASE_URL_HERE.com/php/index.php?".$_SERVER['QUERY_STRING']); 
		header("Connection: close"); 	
	}
?>		
	</body>
</html>