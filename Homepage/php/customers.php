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

	function generatCustomerGrid($tKunde) {
		
		echo '<br/><div class="container"><a href="index.php" class="previous"><button type="button" class="btn btn-lg">&#x3C;&#x3C; Show Menu</button></a></div>';		

		readfile("../html/customersTableHead.html");
		
		foreach($tKunde as $key => $kunde) {
			
			$kunde['cNote'] = trim($kunde['cNote'] , '<br />');
			
			if(strlen($kunde['cNote']) > 50)
			{
				$kunde['cNote'] = substr($kunde['cNote'] , 0, 46) . ' [...]';				
			}
			
			if(stripos($kunde['cNote'], '<br />') !== false)
			{
				$kunde['cNote'] = substr($kunde['cNote'] , 0, stripos($kunde['cNote'] , '<br />')) . ' [...]';				
			}
			
			echo '<tr role="row">';	
				echo '<td>'.$kunde['kCut'].'</td>';			
				echo '<td>'.$kunde['kAge'].'</td>';
				echo '<td>'.$kunde['cGenotype'].'</td>';
				echo '<td>'.$kunde['cAnimal'].'</td>';
				echo '<td>'.$kunde['cCutIdentifier'].'</td>';				
				echo '<td>'.$kunde['dDateMeasurement'].'</td>';
				echo '<td>'.$kunde['cNote'].'</td>';
				echo '<td><a href="details.php?age='.$kunde['kAge'].'&genotype='.$kunde['cGenotype'].'&animal='.$kunde['cAnimal'].'&cutidentifier='.$kunde['cCutIdentifier'].'"><i class="fa fa-pencil"><button type="button" class="btn btn-default">Show Details</button></i></a></td>';
			echo '</tr>';	
		}			

		echo '</tbody>';
		echo '</table>';
		echo '</div>';	
	}
?>