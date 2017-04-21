<?php 
    require("mysql_info.php");

    // Strings must be escaped to prevent SQL injection attack. 
    $name = mysql_real_escape_string($_GET['n']);
    $password = $_GET['p'];
    $hash = $_GET['h']; 

    $secretKey = "afzlearvtoyuurimpeqlsddkfagmhejrk"; //flavourmeldkamer with salt in between
    $real_hash = md5($name . $secretKey . $password);
    
    $error = "";
    if($hash == $real_hash)
    {
        //Check if we can login
        $query = "SELECT * FROM Flavour_Meld WHERE name='$name'";
        $result = mysql_query($query) or die("Query failed: " . mysql_error());
     
        $num_results = mysql_num_rows($result);  
     
        if ($num_results == 0)
        {
            $error = "Invalid username.";
        }

        for($i = 0; $i < $num_results; $i++)
        {
            $row = mysql_fetch_array($result);
            $passwordHash = $row['password'];

            if (password_verify($password, $passwordHash))
            {
                $timestamp = date("Y-m-d H:i:s");

                mysql_query("UPDATE Flavour_Meld SET last_login_timestamp='$timestamp' WHERE name='$name'")
                             or die("Query failed: " . mysql_error());
            }
            else
            {
                $error = "Invalid password.";
            }
        }
    }
    else
    {
        $error = "Hashes do not match. What are you doing, trying to hack this system?";
    }

    echo $error;
?>