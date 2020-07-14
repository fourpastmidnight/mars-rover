import React, { useEffect, useState } from 'react';
import { Button, Card, CardDeck, Col, Container, Row } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import RoverPhotosBanner from './RoverPhotosBanner';
import * as Api from '../../api/marsRoverPhotosApi'

const HomePage = ({numCarouselPhotos}) => {
    const [carouselPhotos, setCarouselPhotos] = useState([]);
    const [cardPhotos, setCardPhotos] = useState([])

    const getRandomRoverPhoto = async (rover) => {
        const roverPhotos = (await Api.getAllRoverPhotos(rover)).photos;
        return roverPhotos.length === 0
            ? null
            : roverPhotos[Math.floor(Math.random() * roverPhotos.length)];
    }

    function getPhotoRoverName(photoUrl) {
        const apiEndpoint = 'rover-photos/download/'
        const roverNameIndex= photoUrl.indexOf(apiEndpoint) + apiEndpoint.length
        const endOfRoverNameIndex = photoUrl.substr(roverNameIndex).lastIndexOf('/') - 11;
        const roverName = photoUrl.substr(roverNameIndex, endOfRoverNameIndex);
        return roverName.replace(/^\w/, c => c.toUpperCase());
    }

    useEffect(() => {
        const fetchRandomPhotos = async () => setCarouselPhotos((await Api.getRandomRoverPhotos(numCarouselPhotos)).photos);

        fetchRandomPhotos();
    }, [numCarouselPhotos])

    useEffect(() => {
        const fetchCardPhotos = async () =>
            setCardPhotos((await Promise.all(["Curiosity", "Opportunity", "Spirit"]
                .map(async r => await getRandomRoverPhoto(r))))
                .filter(p => p !== null));

        fetchCardPhotos();
    }, [])

    return (
        <Container as="main" className="d-flex flex-column h-100">
            <Row className="mx-0 d-block">
                <Col className="px-0">
                    <RoverPhotosBanner id="roverPhotosCarousel" carouselPhotoUrls={carouselPhotos} getPhotoRoverName={getPhotoRoverName} />
                </Col>
            </Row>
            <Row className="d-block flex-grow-1 mx-0" style={{backgroundColor: "#fff8f0"}}>
                <Col>
                    <Container as="section" className="px-0">
                        <Row>
                            <Col>
                                <h1 className="mt-4">NASA Mars Rover Photos Gallery</h1>
                                <p>
                                    Welcome to my gallery of NASA Mars Rover Photos. Here you can find photographs from all three of
                                    NASA's Mars rovers from various dates. Search for a photo by Rover and/or by date, or simply use
                                    the menu to view photos by rover.
                                </p>
                            </Col>
                        </Row>
                        <Row className="pt-4 justify-content-around">
                            <Col>
                                <CardDeck>
                                    {cardPhotos.map(p => {
                                        const roverName = getPhotoRoverName(p);
                                        return (
                                            <Card key={roverName} className="shadow-lg">
                                                <div className="w-100" style={{backgroundColor: "#282828"}}>
                                                    <Card.Img variant="top" src={p} style={{height: "200px", objectFit: "cover"}} />
                                                </div>
                                                <Card.Body>
                                                    <Card.Title>{roverName} Rover Photos</Card.Title>
                                                    <Card.Text>
                                                        See more NASA Mars {roverName} Rover photos
                                                    </Card.Text>
                                                    <Link to={`/${roverName}`} className="stretched-link">
                                                        <Button variant="primary">{roverName} Photos</Button>
                                                    </Link>
                                                </Card.Body>
                                            </Card>
                                        );
                                    })}
                                </CardDeck>
                            </Col>
                        </Row>
                    </Container>
                </Col>
            </Row>
        </Container>
    );
};

HomePage.propTypes = {
    numCarouselPhotos: PropTypes.number.isRequired,
}

export default HomePage;