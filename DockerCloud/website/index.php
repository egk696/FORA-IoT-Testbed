<html>
    <head>
        <title>Hello world</title>
    </head>

    <body>
        <h1>Welcome to my hello world page</h1>
        <ul>
            <?php
            $json = file_get_contents('http://data-integration:80/');
            $obj = json_decode($json);
            $products = $obj->products;
            foreach ($products as $product) {
                echo "<li>$product</li>";
            }
            ?>
        </ul>
    </body>
</html>
