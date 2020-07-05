import React, { useEffect, useState } from 'react';
import { Col, Container, Row } from 'react-bootstrap';
import PropTypes from 'prop-types';
import RoverPhotosBanner from './RoverPhotosBanner';
import * as Api from '../../api/marsRoverPhotosApi'

const HomePage = ({numCarouselPhotos}) => {
    const [carouselPhotos, setCarouselPhotos] = useState([]);

    }

    useEffect(() => {
        const fetchRandomPhotos = async () => setCarouselPhotos((await Api.getRandomRoverPhotos(numCarouselPhotos)).photos);

        fetchRandomPhotos();
    }, [numCarouselPhotos])

    return (
        <Container as="main" className="d-flex flex-column h-100">
            <Row className="mx-0 d-block">
                <Col className="px-0">
                    <RoverPhotosBanner id="roverPhotosCarousel" carouselPhotoUrls={carouselPhotos} getPhotoRoverName={getPhotoRoverName} />
                </Col>
            </Row>
            <Row className="d-block flex-grow-1 mx-0" style={{backgroundColor: "#fff8f0"}}>
                <Col>
                    <h1 className="mt-4">NASA Mars Rover Photos Gallery</h1>
                    <p>
                        Welcome to my gallery of NASA Mars Rover Photos. Here you can find photographs from all three of
                        NASA's Mars rovers from various dates. Search for a photo by Rover and/or by date, or simply use
                        the menu to view photos by rover.
                    </p>
                </Col>
            </Row>
        </Container>
    );
};

HomePage.propTypes = {
    numCarouselPhotos: PropTypes.number.isRequired,
}

export default HomePage;