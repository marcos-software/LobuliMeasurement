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
								
	$cutId         = $_POST['cutid'];
	$resulttype    = $_POST['resulttype'];
	$identifier    = $_POST['cIdentifier'];
	
	$age           = $_POST['age'];
	$genotype      = $_POST['genotype'];
	$animal        = $_POST['animal'];
	$cutidentifier = $_POST['cutidentifier'];

	if
	(
		(is_null($cutId) == false && isset($cutId) && trim($cutId) !== '') &&
		(is_null($resulttype) == false && isset($resulttype) && trim($resulttype) !== '') &&
		(is_null($identifier) == false && isset($identifier) && trim($identifier) !== '')
	)	
	{		
	    echo '<html lang="en">';
		readfile("../html/head.html");	
		require_once('db.php');
			
		$pdoLog                 = getPDO(1);		

		$imageTypeName = '';

		if($resulttype == "Fissure")
		{
			$imageTypeName = 'f';
		}
		if($resulttype == "Lobule")
		{
			$imageTypeName = 'l';
		}			
		
		$statement              = $pdoLog->prepare("SELECT * FROM vImage WHERE kCut = :kCut AND cType = :cType");
		$statement->execute(array('kCut' => $cutId, 'cType' => $imageTypeName . $identifier . ' image'));
		$vImageAnalyzed         = $statement->fetch();
		
		$statement              = $pdoLog->prepare("SELECT cKey AS cKey, CONCAT(cValue, ' ', cUnit) AS cValue FROM vResult WHERE kCut = :kCut AND cResultType = :cResultType AND cIdentifier = :cIdentifier");
		$statement->execute(array('kCut' => $cutId, 'cResultType' => $resulttype, 'cIdentifier' => $identifier));
		$vResults               = $statement->fetchAll();
		
	
	
	// readfile("../html/details.html");	
	
	$lastpage = 'details.php?age='.$age.'&genotype='.$genotype.'&animal='.$animal.'&cutidentifier='.$cutidentifier;		
	$lastpageName = 'Details';
	
	echo '<body>
	<div id="masthead">  
	  <div class="container">
	  <br/>
	 <a href="'.$lastpage.'" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show '.$lastpageName.'</button></a>
	 <br/>
			  <h1> '.$resulttype .' '.$identifier.' Details
				<p class="lead">Slice # '.$cutId .'</p>
			  </h1>
		  <div class="row">
			<div class="col-md-6">			
				<table class="table table-striped">
					<tbody>';
					
					foreach($vResults as $key => $row) 
				{
					$key = $row['cKey'];
					$value = $row['cValue'];
					
					echo '
					<tr>
						<th scope="row">'.$key.'</th>					
						<td>'.$value.'</td>			
					</tr>';	
				}
					
					
					echo'	
					  </tbody>
				</table>
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
				</div>
			</div>		  
	  </div><!--/container-->
	</div><!--/masthead-->
	<!--main-->
	<!--/container-->';


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